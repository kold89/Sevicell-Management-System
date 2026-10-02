using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para installmentsduePage.xaml
    /// </summary>
    public partial class installmentsduePage : Page
    {
        private readonly CreditContractsServices ContractsSevices = new CreditContractsServices();
        private List<CobroItemDto> _todasLasCuotas = new();
        private string _filtroActivo = null;
        private List<CobroItemDto> _filtradas = new();

        private readonly ReciboPrinterService _printerService = new ReciboPrinterService();
        private ReciboPagoCuotaDto _ultimoReciboImpreso;
        public installmentsduePage()
        {
            InitializeComponent();
            _ = LoadCobrosDataAsync();
        }

        private async Task LoadCobrosDataAsync(bool reiniciarPagina = true)
        {
            var result = await ContractsSevices.GetCobrosDashboardAsync();
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _todasLasCuotas = result.Data;
            RenderTarjetas();
            RenderTabla(reiniciarPagina);
        }
        private void RenderTabla(bool reiniciarPagina = true)
        {
            var query = _filtroActivo == null
                ? _todasLasCuotas
                : _todasLasCuotas.Where(x => x.UrgencyGroup == _filtroActivo).ToList();

            // ThenBy: orden estable para que Skip/Take no repita ni salte filas
            _filtradas = query
                .OrderBy(x => x.DueDate)
                .ThenBy(x => x.InstallmentId)
                .ToList();

            Paginador.Configurar(_filtradas.Count, reiniciarPagina ? 1 : Paginador.PaginaActual);
            MostrarPagina();
        }

        private void MostrarPagina()
        {
            if (_filtradas.Count == 0)
            {
                DgCobros.ItemsSource = null;
                TxtResumen.Text = "No hay cuotas para mostrar.";
                Paginador.Visibility = Visibility.Collapsed;
                return;
            }

            Paginador.Visibility = Visibility.Visible;

            int tam = Paginador.RegistrosPorPagina;
            int salto = (Paginador.PaginaActual - 1) * tam;
            var pagina = _filtradas.Skip(salto).Take(tam).ToList();

            DgCobros.ItemsSource = pagina;
            TxtResumen.Text = $"Mostrando {salto + 1}–{salto + pagina.Count} de {_filtradas.Count} cuota(s).";
        }

        private void Paginador_PaginaCambiada(object sender, EventArgs e) => MostrarPagina();
        private void RenderTarjetas()
        {
            var tarjetas = new List<CardStatusInstallment>
            {
                new() { Code = "OVERDUE", Label = "Vencidas", DarkColor = "#791F1F",
                        Count = _todasLasCuotas.Count(x => x.UrgencyGroup == "OVERDUE") },
                new() { Code = "TODAY", Label = "Vencen hoy", DarkColor = "#854F0B",
                        Count = _todasLasCuotas.Count(x => x.UrgencyGroup == "TODAY") },
                new() { Code = "WEEK", Label = "Esta semana", DarkColor = "#0C447C",
                        Count = _todasLasCuotas.Count(x => x.UrgencyGroup == "WEEK") }
            };
            IcTarjetas.ItemsSource = tarjetas;
        }

        private void TarjetaEstado_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border || border.Tag is not string code) return;
            _filtroActivo = _filtroActivo == code ? null : code;
            RenderTabla();
        }

        private async void BtnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int installmentId) return;
            await AbrirDialogoDePago(installmentId, esPagoTotal: true);
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }

        private async void BtnCobroParcial_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int installmentId) return;
            await AbrirDialogoDePago(installmentId, esPagoTotal: false);
        }
        private async Task AbrirDialogoDePago(int installmentId, bool esPagoTotal)
        {
            var cuota = _todasLasCuotas.FirstOrDefault(x => x.InstallmentId == installmentId);
            if (cuota == null) return;

            var saldo = cuota.ExpectedAmount - cuota.PaidAmount;

            var dialog = new PagoAbonoDialog(saldo, esPagoTotal) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;

            var result = await ContractsSevices.RegisterPaymentAsync(installmentId, dialog.Monto, DateTime.Now);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ToastService.ShowSuccess(result.Message); // ya viene "Pago registrado" o "Abono registrado" desde el service
            try
            {
                _printerService.ImprimirReciboPago(result.Data, "POS-58-Series");
                _ultimoReciboImpreso = result.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"El pago se registró correctamente, pero no se pudo imprimir el recibo.\n\nMotivo: {ex.Message}\n\nPuede usar 'Reimprimir último recibo'.",
                    "Aviso de impresión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            await LoadCobrosDataAsync(reiniciarPagina: false);
        }
    }
}
