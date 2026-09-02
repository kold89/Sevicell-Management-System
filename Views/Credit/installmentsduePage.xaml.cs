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

        private readonly ReciboPrinterService _printerService = new ReciboPrinterService();
        private ReciboPagoCuotaDto _ultimoReciboImpreso;
        public installmentsduePage()
        {
            InitializeComponent();
            _ = LoadCobrosDataAsync();
        }

        private async Task LoadCobrosDataAsync()
        {
            var result = await ContractsSevices.GetCobrosDashboardAsync();
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _todasLasCuotas = result.Data;
            RenderTarjetas();
            RenderTabla();
        }

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

        private void RenderTabla()
        {
            var lista = _filtroActivo == null
                ? _todasLasCuotas
                : _todasLasCuotas.Where(x => x.UrgencyGroup == _filtroActivo).ToList();

            DgCobros.ItemsSource = lista.OrderBy(x => x.DueDate).ToList();
        }

        private void TarjetaEstado_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border || border.Tag is not string code) return;
            _filtroActivo = _filtroActivo == code ? null : code;
            RenderTabla();
        }

        private async void BtnCobrar_Click(object sender, RoutedEventArgs e)
        {
            //if (sender is not Button btn || btn.Tag is not int installmentId) return;

            //var confirm = MessageBox.Show("¿Confirmar el pago de esta cuota en efectivo?", "Confirmar pago",
            //                                MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (confirm != MessageBoxResult.Yes) return;

            //var result = await ContractsSevices.RegisterPaymentAsync(installmentId, DateTime.Now);
            //if (!result.Success)
            //{
            //    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //MessageBox.Show("Pago registrado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            //await LoadCobrosDataAsync(); // recarga todo -- la cuota pagada desaparece sola de la lista
            if (sender is not Button btn || btn.Tag is not int installmentId) return;

            var confirm = MessageBox.Show("¿Confirmar el pago de esta cuota en efectivo?", "Confirmar pago",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            var result = await ContractsSevices.RegisterPaymentAsync(installmentId, DateTime.Now);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //MessageBox.Show("Pago registrado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            ToastService.ShowSuccess("Pago registrado exitosamente.");

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

            await LoadCobrosDataAsync();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}
