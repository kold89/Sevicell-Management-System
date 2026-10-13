using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    public partial class suppliersPage : Page
    {
        private readonly SupplierServices supplierServices = new SupplierServices();
        private List<SupplierDto> _allSuppliers = new();
        private bool _updatingPager;

        public suppliersPage()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadData();
        }

        // Paso 1 y 3: trae los datos, los guarda y configura el paginador
        private async Task LoadData()
        {
            try
            {
                var result = await supplierServices.listSupplierForGrid();

                if (!result.Success || result.Data == null)
                {
                    _allSuppliers = new List<SupplierDto>();
                    ToastService.ShowInfo(result?.Message ?? "Error al cargar los proveedores.");
                }
                else
                {
                    _allSuppliers = result.Data;
                }

                // Conserva la página actual, pero nunca una que ya no exista
                int size = Paginador.RegistrosPorPagina;
                int totalPages = Math.Max(1, (int)Math.Ceiling(_allSuppliers.Count / (double)size));
                int page = Math.Clamp(Paginador.PaginaActual, 1, totalPages);

                _updatingPager = true;
                Paginador.Configurar(_allSuppliers.Count, page);
                _updatingPager = false;

                ShowPage();
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error inesperado: " + ex.Message);
            }
        }

        // Paso 2: corta el pedazo que toca y lo muestra
        private void ShowPage()
        {
            int size = Paginador.RegistrosPorPagina;
            int skip = (Paginador.PaginaActual - 1) * size;

            var pageItems = _allSuppliers.Skip(skip).Take(size).ToList();
            dgSupplier.ItemsSource = pageItems;

            bool empty = _allSuppliers.Count == 0;
            Paginador.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
            TxtResumen.Text = empty
                ? string.Empty
                : $"Mostrando {skip + 1}–{skip + pageItems.Count} de {_allSuppliers.Count} proveedor(es).";
        }

        // Paso 4: el usuario pulsó una flecha del paginador
        private void Paginador_PaginaCambiada(object sender, EventArgs e)
        {
            if (_updatingPager) return;
            ShowPage();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private async void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            var modal = new AddOrEditSupplier { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true)
                await LoadData();
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: SupplierDto row }) return;

            var supplier = supplierServices.GetSupplierById(row.id);

            var modal = new AddOrEditSupplier(supplier.Data) { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true)
                await LoadData();
        }

        private void BtnSee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: SupplierDto row }) return;

            var supplier = supplierServices.SearchSupplierById(row.id);

            var modal = new DialogSupplierForSee(supplier.Data) { Owner = Window.GetWindow(this) };
            modal.ShowDialog();
        }
    }
}