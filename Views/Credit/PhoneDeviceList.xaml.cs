using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views.Credit
{
    public partial class PhoneDeviceList : Page
    {
        public class statusPhone
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class UnidadListItem
        {
            public int ProductUnitId { get; set; }
            public string Name { get; set; }
            public string Imei { get; set; }
            public string Imei2 { get; set; }
            public string Colour { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
        }

        public ObservableCollection<UnidadListItem> Unidades { get; set; } = new();

        public PhoneDeviceList()
        {
            InitializeComponent();
            DataContext = this;
            LoadTypeInovice();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await BuscarUnidades();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void LoadTypeInovice()
        {
            try
            {
                List<statusPhone> status = new List<statusPhone>
                {
                    new statusPhone { id = 0, name = "--Todos--" },
                    new statusPhone { id = 1, name = "Disponibles" },
                    new statusPhone { id = 2, name = "Vendidos" }
                };
                CboFilteStatus.ItemsSource = status;
                CboFilteStatus.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error al cargar tipo de estado: " + ex.Message);
            }
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            await BuscarUnidades();
        }

        private async Task BuscarUnidades()
        {
            using (var db = new SevicellDbContext())
            {
                var query = db.ProductUnits
                    .Include(u => u.Product)
                    .AsQueryable();

                if (CboFilteStatus.SelectedValue is int statusId)
                {
                    if (statusId == 1)
                        query = query.Where(u => u.Status == "Disponible");
                    else if (statusId == 2)
                        query = query.Where(u => u.Status == "Vendido");
                }

                var filtro = TxtFiltroNumero.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    query = query.Where(u =>
                        u.Imei.Contains(filtro) ||
                        u.Imei2.Contains(filtro) ||
                        (u.Model != null && u.Model.Contains(filtro)));
                }

                var lista = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UnidadListItem
                    {
                        ProductUnitId = u.Id,
                        Name = u.Product.Name,
                        Imei = u.Imei,
                        Imei2 = u.Imei2,
                        Colour = u.Colour,
                        Model = u.Model,
                        Status = u.Status
                    })
                    .ToListAsync();

                Unidades.Clear();
                foreach (var item in lista)
                    Unidades.Add(item);
            }
        }

        private async void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not UnidadListItem unidad)
                return;

            if (unidad.Status != "Vendido")
            {
                ToastService.ShowInfo("Este dispositivo está disponible para la venta.");
                return;
            }

            using (var db = new SevicellDbContext())
            {
                var contract = await db.Contracts
                    .Include(c => c.Client)
                    .Include(c => c.ProductUnit)
                        .ThenInclude(pu => pu.Product)
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.ProductUnitId == unidad.ProductUnitId);

                if (contract == null)
                {
                    ToastService.ShowError("No se encontró un contrato para este dispositivo.");
                    return;
                }

                var modal = new SaleDetailWindow(
                    numeroContrato: $"CR-{contract.Id:D4}",
                    clienteNombre: $"{contract.Client.Name} {contract.Client.LastName}",
                    details: $"Dirección: {contract.Client.Address}  Tel: {contract.Client.Phone}",
                    fechaInicio: contract.CreatedAt,
                    dispositivo: $"{contract.ProductUnit.Product.Name} ({contract.ProductUnit.Model})",
                    imei: contract.ProductUnit.Imei,
                    estado: contract.Status.Code
                );
                modal.Owner = Window.GetWindow(this);
                modal.ShowDialog();
            }
        }
    }
}