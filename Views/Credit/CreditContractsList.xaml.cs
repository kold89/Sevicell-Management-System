using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    public partial class CreditContractsList : Page
    {
        // ---------- Modelos de la vista ----------
        public class EstadoFiltro
        {
            public string Id { get; set; }   // "" = todos
            public string Name { get; set; }
        }
        private readonly ReciboPrinterService _printerService = new ReciboPrinterService();

        private enum EstadoLista
        {
            Resultados,
            SinResultados,  // hay filtros y nada coincide
            SinContratos,   // sin filtros y no hay contratos
            Error
        }

        // ---------- Campos ----------
        private static readonly Brush BrushTextoNormal = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B));
        private static readonly Brush BrushPlaceholder = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
        private static readonly Brush BrushTituloNormal = new SolidColorBrush(Color.FromRgb(0x47, 0x55, 0x69));
        private static readonly Brush BrushError = new SolidColorBrush(Color.FromRgb(0xC0, 0x39, 0x2B));

        private readonly CreditContractsServices ContractsSevices = new CreditContractsServices();
        private readonly ContractsDocumentGenerator documentGenerator = new ContractsDocumentGenerator();
        private List<creditContractsDTO> _allContracts = new();
        private DateTime? _fechaDesde;
        private DateTime? _fechaHasta;
        private bool _isSearching;

        // ---------- Ciclo de vida ----------
        public CreditContractsList()
        {
            InitializeComponent();
            LoadStatusFilter();
        }

        // Se dispara cada vez que la página se muestra (incluye volver desde "Nuevo Contrato"),
        // así la lista siempre está actualizada.
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await BuscarContratos(actualizarVencimientos: true);
        }

        private void LoadStatusFilter()
        {
            CboFilterStatus.ItemsSource = new List<EstadoFiltro>
            {
                new EstadoFiltro { Id = "",           Name = "--Todos--" },
                new EstadoFiltro { Id = "PENDIENTE",  Name = "Pendiente" },
                new EstadoFiltro { Id = "ACTIVO",     Name = "Activo" },
                new EstadoFiltro { Id = "VENCIDO",    Name = "Vencido" },
                new EstadoFiltro { Id = "CANCELADO",  Name = "Cancelado" },
                new EstadoFiltro { Id = "COMPLETADO", Name = "Completado" }
            };
            CboFilterStatus.SelectedValue = "";
        }

        // ---------- Navegación ----------
        private void BtnAddContract_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreditContracts());
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        // ---------- Filtros: botones ----------
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            await BuscarContratos();
        }

        private async void TxtFiltroBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await BuscarContratos();
        }

        private async void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // SelectedDate = null dispara SelectedDatesChanged, que actualiza texto, color y campos
            CalFechaDesde.SelectedDate = null;
            CalFechaHasta.SelectedDate = null;
            CboFilterStatus.SelectedValue = "";
            TxtFiltroBusqueda.Clear();

            await BuscarContratos();
        }

        // ---------- Filtros: fechas ----------
        private void BorderFechaDesde_Click(object sender, MouseButtonEventArgs e)
        {
            PopupCalendarioDesde.IsOpen = true;
        }

        private void BorderFechaHasta_Click(object sender, MouseButtonEventArgs e)
        {
            PopupCalendarioHasta.IsOpen = true;
        }

        private void CalFechaDesde_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _fechaDesde = CalFechaDesde.SelectedDate;
            MostrarFecha(_fechaDesde, TxtFechaDesdeSeleccionada, BtnClearDesde, PopupCalendarioDesde);
        }

        private void CalFechaHasta_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _fechaHasta = CalFechaHasta.SelectedDate;
            MostrarFecha(_fechaHasta, TxtFechaHastaSeleccionada, BtnClearHasta, PopupCalendarioHasta);
        }

        private void BtnClearDesde_Click(object sender, RoutedEventArgs e)
        {
            CalFechaDesde.SelectedDate = null;
        }

        private void BtnClearHasta_Click(object sender, RoutedEventArgs e)
        {
            CalFechaHasta.SelectedDate = null;
        }

        private static void MostrarFecha(DateTime? fecha, TextBlock texto, Button botonQuitar, Popup popup)
        {
            texto.Text = fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy") : "Todas";
            texto.Foreground = fecha.HasValue ? BrushTextoNormal : BrushPlaceholder;
            botonQuitar.Visibility = fecha.HasValue ? Visibility.Visible : Visibility.Collapsed;
            popup.IsOpen = false;
        }

        // Fix conocido de WPF: el Calendar se queda con la captura del mouse tras elegir un día
        // y el siguiente clic fuera del popup se "pierde".
        private void Calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
                Mouse.Capture(null);
        }

        // ---------- Búsqueda ----------
        private async Task BuscarContratos(bool actualizarVencimientos = false)
        {
            // Validación antes de bloquear nada
            if (_fechaDesde.HasValue && _fechaHasta.HasValue && _fechaDesde.Value.Date > _fechaHasta.Value.Date)
            {
                ToastService.ShowInfo("La fecha \"Desde\" no puede ser mayor que la fecha \"Hasta\".");
                return;
            }

            // Evita búsquedas solapadas (Loaded + clic, doble clic, Enter + clic)
            if (_isSearching) return;
            _isSearching = true;

            BtnBuscar.IsEnabled = false;
            BtnBuscar.Content = "Buscando...";
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                // Capturamos los filtros antes de cualquier await para que no cambien a mitad de la búsqueda
                DateTime? desde = _fechaDesde?.Date;
                DateTime? hasta = _fechaHasta?.Date;
                string estado = CboFilterStatus.SelectedValue as string;
                string texto = TxtFiltroBusqueda.Text?.Trim();

                bool hayFiltros = desde.HasValue || hasta.HasValue
                                  || !string.IsNullOrEmpty(estado)
                                  || !string.IsNullOrWhiteSpace(texto);

                // Marca como VENCIDOS los contratos con cuotas atrasadas.
                // Si falla, no debe impedir que se muestre el listado.
                if (actualizarVencimientos)
                {
                    try
                    {
                        await ContractsSevices.ActualizarVencimientosAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("No se pudieron actualizar los vencimientos: " + ex.Message);
                    }
                }

                var result = await ContractsSevices.ListCreditContractsDtoAsync();

                if (!result.Success || result.Data == null)
                    throw new InvalidOperationException("No se pudo obtener el listado de contratos.");

                _allContracts = result.Data;

                // Filtros en memoria sobre la lista cargada
                IEnumerable<creditContractsDTO> query = _allContracts;

                if (!string.IsNullOrEmpty(estado))
                    query = query.Where(c => string.Equals(c.Status, estado, StringComparison.OrdinalIgnoreCase));

                if (desde.HasValue)
                    query = query.Where(c =>
                    {
                        var f = (DateTime?)c.CreatedAt;
                        return f.HasValue && f.Value.Date >= desde.Value;
                    });

                if (hasta.HasValue)
                    query = query.Where(c =>
                    {
                        var f = (DateTime?)c.CreatedAt;
                        return f.HasValue && f.Value.Date <= hasta.Value;
                    });

                if (!string.IsNullOrWhiteSpace(texto))
                    query = query.Where(c =>
                        Contiene(c.CustomerName, texto) ||
                        //Contiene(c.DniCustomer?.ToString(), texto) ||
                        c.ContractNumber.ToString().Contains(texto));

                var lista = query
                    .OrderByDescending(c => (DateTime?)c.CreatedAt)
                    .ToList();

                DgContracts.ItemsSource = lista;

                if (lista.Count > 0)
                {
                    MostrarEstado(EstadoLista.Resultados);
                    TxtResumen.Text = $"{lista.Count} contrato(s) encontrado(s).";
                }
                else
                {
                    MostrarEstado(hayFiltros ? EstadoLista.SinResultados : EstadoLista.SinContratos);
                }
            }
            catch (Exception ex)
            {
                DgContracts.ItemsSource = null;
                MostrarEstado(EstadoLista.Error);
                ToastService.ShowError("Error al cargar los contratos: " + ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                BtnBuscar.Content = "🔍 Buscar";
                BtnBuscar.IsEnabled = true;
                _isSearching = false;
            }
        }

        private static bool Contiene(string valor, string texto)
        {
            return !string.IsNullOrEmpty(valor) && valor.Contains(texto, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Único punto que decide qué se ve: el DataGrid o el panel de vacío/error.
        /// </summary>
        private void MostrarEstado(EstadoLista estado)
        {
            bool mostrarGrid = estado == EstadoLista.Resultados;

            DgContracts.Visibility = mostrarGrid ? Visibility.Visible : Visibility.Collapsed;
            PnlEmptyState.Visibility = mostrarGrid ? Visibility.Collapsed : Visibility.Visible;

            if (mostrarGrid)
                return;

            TxtResumen.Text = string.Empty;
            TxtEmptyTitle.Foreground = estado == EstadoLista.Error ? BrushError : BrushTituloNormal;

            switch (estado)
            {
                case EstadoLista.SinContratos:
                    TxtEmptyTitle.Text = "Aún no hay contratos registrados";
                    TxtEmptySub.Text = "Cuando registres un contrato de crédito aparecerá aquí.";
                    break;

                case EstadoLista.SinResultados:
                    TxtEmptyTitle.Text = "No se encontraron contratos";
                    TxtEmptySub.Text = "Intenta cambiar los filtros de búsqueda o el criterio ingresado.";
                    break;

                case EstadoLista.Error:
                    TxtEmptyTitle.Text = "Error al consultar la base de datos";
                    TxtEmptySub.Text = "Ocurrió un problema de conexión. Intente buscar nuevamente.";
                    break;
            }
        }

        // ---------- Acciones por fila ----------
        private void BtnVerCuotas_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int contractId) return;
            var ventana = new CuotasWindow(contractId) { Owner = Window.GetWindow(this) };
            ventana.ShowDialog();
        }

        private void BtnVerHistorial_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int contractId) return;
            var ventana = new HistorialPagosWindow(contractId) { Owner = Window.GetWindow(this) };
            ventana.ShowDialog();
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int contractId) return;

            try
            {
                // Se busca en la lista completa cargada (no en la filtrada): no se vuelve a golpear la BD
                var contractSelection = _allContracts.FirstOrDefault(c => c.ContractNumber == contractId);

                if (contractSelection == null)
                {
                    ToastService.ShowError("No se encontró el contrato.");
                    return;
                }

                var cuotasResult = await ContractsSevices.GetDebtInstalmentAsync(contractId);
                if (!cuotasResult.Success || cuotasResult.Data == null || !cuotasResult.Data.Any())
                {
                    ToastService.ShowWarning("No se encontraron cuotas para este contrato.");
                    return;
                }

                var cuotas = cuotasResult.Data;
                var cuotasOrdenadas = cuotas.OrderBy(c => c.DueDate).ToList();
                var primeraCuota = cuotasOrdenadas.First();
                var ultimaCuota = cuotasOrdenadas.Last();

                SellerInfo  seller = new SellerInfo(contractSelection.SellerName, contractSelection.DniSeller, contractSelection.empresa);
                CustomerInfo customer = new CustomerInfo(contractSelection.CustomerName, contractSelection.DniCustomer, contractSelection.CustomerAddress);
                ProductInfo product = new ProductInfo(contractSelection.ProductName, contractSelection.Brand, contractSelection.model, 
                                            contractSelection.colour, contractSelection.Imei, contractSelection.Imei2);
                PaymentInfo payment = new PaymentInfo(contractSelection.PriceSales, contractSelection.DownPayment, cuotas.Count, primeraCuota.ExpectedAmount, "mensual");


                var BuildertempContractDto = new BuilderContractDto();

                var dto = BuildertempContractDto.AddSeller(seller)
                                .AddCustomer(customer)
                                .AddProduct(product)
                                .AddPaymentStructure(payment)
                                .AddValidityAndPlace(primeraCuota, ultimaCuota)
                                .buildContratoDto();
                documentGenerator.GenerarContratoDocumento(dto);
                //ContractsSevices.GenerarContratoDocumento(dto);
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error al generar el contrato: " + ex.Message);
            }
        }

        private async void BtnAdelanto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int contractId) return;

            var contractSelection = (DgContracts.ItemsSource as IEnumerable<creditContractsDTO>)
                ?.FirstOrDefault(c => c.ContractNumber == contractId);
            if (contractSelection == null) return;

            var cuotasResult = await ContractsSevices.GetDebtInstalmentAsync(contractId);
            if (!cuotasResult.Success) return;

            var pendientes = cuotasResult.Data.Where(c => c.SaldoPendiente > 0).ToList();
            if (pendientes.Count == 0)
            {
                MessageBox.Show("Este contrato no tiene saldo pendiente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new AdelantoPagoDialog(contractId, contractSelection.CustomerName, pendientes)
            { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;

            var result = await ContractsSevices.RegisterAdvancePaymentAsync(contractId, dialog.Monto, DateTime.Now);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ToastService.ShowSuccess(result.Message);
            if (!ImpresoraExiste("POS-58-Series"))
            {
                MessageBox.Show(
                    "El adelanto se registró correctamente, pero no se encontró la impresora 'POS-58-Series' para imprimir el recibo.",
                    "Aviso de impresión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    _printerService.ImprimirReciboAdelanto(result.Data, "POS-58-Series");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"El adelanto se registró correctamente, pero no se pudo imprimir el recibo.\n\nMotivo: {ex.Message}",
                        "Aviso de impresión", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            BuscarContratos();
        }

        private bool ImpresoraExiste(string nombreImpresora)
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer.Equals(nombreImpresora, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}