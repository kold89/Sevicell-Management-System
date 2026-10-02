using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;
using WpfApp1.Models.Enums;
using WpfApp1.Services;

namespace WpfApp1.Views.Credit
{
    public partial class PhoneDeviceList : Page
    {
        // ---------- Modelos de la vista ----------
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

        private enum EstadoLista
        {
            Resultados,
            SinResultados,   // hay filtros aplicados y no coincide nada
            SinDispositivos, // sin filtros y la tabla está vacía
            Error
        }

        // ---------- Constantes y campos ----------
        // Filtros de la última búsqueda: el paginador usa ESTOS, no lo que haya en pantalla
        private int _statusAplicado;
        private string _filtroAplicado;
        private static readonly Brush BrushNormal = new SolidColorBrush(Color.FromRgb(0x47, 0x55, 0x69));
        private static readonly Brush BrushError = new SolidColorBrush(Color.FromRgb(0xC0, 0x39, 0x2B));

        private bool _isSearching;

        public ObservableCollection<UnidadListItem> Unidades { get; set; } = new();

        // ---------- Ciclo de vida ----------
        public PhoneDeviceList()
        {
            InitializeComponent();
            DataContext = this;
            LoadStatusFilter();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await BuscarUnidades();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void LoadStatusFilter()
        {
            CboFilterStatus.ItemsSource = new List<statusPhone>
            {
                new statusPhone { id = 0, name = "--Todos--" },
                new statusPhone { id = 1, name = "Disponibles" },
                new statusPhone { id = 2, name = "Vendidos" },
                new statusPhone { id = 3, name = "Reservados" }
            };
            CboFilterStatus.SelectedValue = 0;
        }

        // ---------- Búsqueda ----------
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            await BuscarUnidades();
        }

        private async void TxtFiltroNumero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await BuscarUnidades();
        }

        private async Task BuscarUnidades(bool nuevaBusqueda = true)
        {
            // Evita búsquedas solapadas (doble clic, Loaded + clic, Enter + clic)
            if (_isSearching) return;
            _isSearching = true;

            BtnBuscar.IsEnabled = false;
            BtnBuscar.Content = "Buscando...";
            Paginador.IsEnabled = false;   // evita clics en las flechas mientras carga
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                // Solo una búsqueda nueva lee los filtros de pantalla; un cambio de página reutiliza los anteriores
                if (nuevaBusqueda)
                {
                    _statusAplicado = CboFilterStatus.SelectedValue is int id ? id : 0;
                    _filtroAplicado = TxtFiltroNumero.Text?.Trim();
                }

                int statusId = _statusAplicado;
                string filtro = _filtroAplicado;
                bool hayFiltros = statusId != 0 || !string.IsNullOrWhiteSpace(filtro);

                using var db = new SevicellDbContext();

                IQueryable<ProductUnit> query = db.ProductUnits.AsNoTracking();

                var disponible = EProductUnitStatus.Disponible.ToDbValue();
                var vendido = EProductUnitStatus.Vendido.ToDbValue();

                switch (statusId)
                {
                    case 1: query = query.Where(u => u.Status == disponible); break;
                    case 2: query = query.Where(u => u.Status == vendido); break;
                    case 3: query = query.Where(u => u.Status == "Reservado"); break;
                }
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    query = query.Where(u =>
                        (u.Imei != null && u.Imei.Contains(filtro)) ||
                        (u.Imei2 != null && u.Imei2.Contains(filtro)) ||
                        (u.Model != null && u.Model.Contains(filtro)));
                }

                int total = await query.CountAsync();

                Paginador.Configurar(total, nuevaBusqueda ? 1 : Paginador.PaginaActual);

                int tam = Paginador.RegistrosPorPagina;
                int salto = (Paginador.PaginaActual - 1) * tam;

                var lista = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .ThenByDescending(u => u.Id)   // orden estable para Skip/Take
                    .Skip(salto)
                    .Take(tam)
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

                if (total > 0)
                {
                    MostrarEstado(EstadoLista.Resultados);
                    TxtResumen.Text = $"Mostrando {salto + 1}–{salto + lista.Count} de {total} dispositivo(s).";
                }
                else
                {
                    MostrarEstado(hayFiltros ? EstadoLista.SinResultados : EstadoLista.SinDispositivos);
                }
            }
            catch (Exception ex)
            {
                Unidades.Clear();
                MostrarEstado(EstadoLista.Error);
                ToastService.ShowError("Error al cargar los dispositivos: " + ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                BtnBuscar.Content = "🔍 Buscar";
                BtnBuscar.IsEnabled = true;
                Paginador.IsEnabled = true;
                _isSearching = false;
            }
        }

        private async void Paginador_PaginaCambiada(object sender, EventArgs e)
        {
            await BuscarUnidades(nuevaBusqueda: false);
        }
        /// <summary>
        /// Único punto que decide qué se ve: el DataGrid o el panel de estado vacío/error.
        /// </summary>
        private void MostrarEstado(EstadoLista estado)
        {
            bool mostrarGrid = estado == EstadoLista.Resultados;
            Paginador.Visibility = mostrarGrid ? Visibility.Visible : Visibility.Collapsed;
            DgUnidades.Visibility = mostrarGrid ? Visibility.Visible : Visibility.Collapsed;
            PnlEmptyState.Visibility = mostrarGrid ? Visibility.Collapsed : Visibility.Visible;

            if (mostrarGrid)
                return;

            TxtResumen.Text = string.Empty;
            TxtEmptyTitle.Foreground = estado == EstadoLista.Error ? BrushError : BrushNormal;

            switch (estado)
            {
                case EstadoLista.SinDispositivos:
                    TxtEmptyTitle.Text = "Aún no hay dispositivos registrados";
                    TxtEmptySub.Text = "Cuando registres dispositivos aparecerán aquí.";
                    break;

                case EstadoLista.SinResultados:
                    TxtEmptyTitle.Text = "No se encontraron dispositivos";
                    TxtEmptySub.Text = "Intenta cambiar los filtros de búsqueda o el criterio ingresado.";
                    break;

                case EstadoLista.Error:
                    TxtEmptyTitle.Text = "Error al consultar la base de datos";
                    TxtEmptySub.Text = "Ocurrió un problema de conexión. Intente buscar nuevamente.";
                    break;
            }
        }

        // ---------- Detalle ----------
        private async void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not UnidadListItem unidad)
                return;

            if (unidad.Status != "Vendido")
            {
                ToastService.ShowInfo("Este dispositivo está disponible para la venta.");
                return;
            }

            try
            {
                using var db = new SevicellDbContext();

                var contract = await db.Contracts
                    .AsNoTracking()
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

                string cliente = contract.Client != null
                    ? $"{contract.Client.Name} {contract.Client.LastName}".Trim()
                    : "Cliente no disponible";

                string detalleCliente = contract.Client != null
                    ? $"Dirección: {contract.Client.Address}  Tel: {contract.Client.Phone}"
                    : string.Empty;

                var pu = contract.ProductUnit;
                string dispositivo = $"{pu?.Product?.Name} ({pu?.Model})";

                var modal = new SaleDetailWindow(
                    numeroContrato: $"CR-{contract.Id:D4}",
                    clienteNombre: cliente,
                    details: detalleCliente,
                    fechaInicio: contract.CreatedAt,
                    dispositivo: dispositivo,
                    imei: pu?.Imei,
                    estado: contract.Status?.Code ?? "N/D"
                );
                modal.Owner = Window.GetWindow(this);
                modal.ShowDialog();
            }
            catch (Exception ex)
            {
                // Dentro de un async void una excepción sin capturar cierra la aplicación
                ToastService.ShowError("Error al cargar el detalle del dispositivo: " + ex.Message);
            }
        }
    }
}
