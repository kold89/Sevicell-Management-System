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


        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
        private async void BtnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int contractId) return;
            await RegistrarPagoDelContratoAsync(contractId);
        }

        private async Task RegistrarPagoDelContratoAsync(int contractId)
        {
            var cuotas = await ContractsSevices.GetDebtInstalmentAsync(contractId);
            if (!cuotas.Success || cuotas.Data == null)
            {
                ToastService.ShowError(cuotas.Message);
                return;
            }

            var pendientes = cuotas.Data.Where(c => c.SaldoPendiente > 0).ToList();
            if (pendientes.Count == 0)
            {
                ToastService.ShowInfo("Este contrato no tiene saldo pendiente.");
                await LoadCobrosDataAsync(reiniciarPagina: false);
                return;
            }

            var cliente = _todasLasCuotas.FirstOrDefault(x => x.ContractId == contractId)?.ClientName ?? "";

            var dialog = new RegistrarPagoDialog(contractId, cliente, pendientes)
            { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;

            var result = await ContractsSevices.RegistrarPagoAsync(contractId, dialog.Monto, DateTime.Now);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ToastService.ShowSuccess(result.Message);

            var error = ImpresionRecibos.Imprimir(result.Data);
            if (error != null)
                MessageBox.Show(
                    $"El pago se registró correctamente, pero no se pudo imprimir el recibo.\n\nMotivo: {error}",
                    "Aviso de impresión", MessageBoxButton.OK, MessageBoxImage.Warning);

            await LoadCobrosDataAsync(reiniciarPagina: false);
        }
    }
}
