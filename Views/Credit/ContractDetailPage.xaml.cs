using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    public partial class ContractDetailPage : Page
    {
        private  creditContractsDTO _contrato;
        private readonly CreditContractsServices _service = new();

        public ContractDetailPage(creditContractsDTO contrato)
        {
            InitializeComponent();
            _contrato = contrato;
            PintarResumen();
        }

        private void PintarResumen()
        {
            TxtTitulo.Text = $"Contrato N° {_contrato.ContractNumber}";
            TxtSubtitulo.Text = $"{_contrato.CustomerName} · {_contrato.ProductName}";
            TxtDeuda.Text = $"L. {_contrato.TotalDebt:N2}";
            TxtPendiente.Text = $"L. {_contrato.Balance:N2}";
            TxtEstado.Text = _contrato.Status;
            TxtCreado.Text = _contrato.CreatedAt.ToString("dd/MM/yyyy");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarCuotasAsync();
            await VistaRecibos.CargarAsync(_contrato.ContractNumber);
        }
        private async Task CargarCuotasAsync()
        {
            var result = await _service.GetDebtInstalmentAsync(_contrato.ContractNumber);

            if (!result.Success || result.Data == null)
            {
                DgCuotas.ItemsSource = null;
                ToastService.ShowError(result.Message);
                return;
            }

            DgCuotas.ItemsSource = result.Data;
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
        private async void BtnRegistrarPago_Click(object sender, RoutedEventArgs e)
        {
            var cuotas = await _service.GetDebtInstalmentAsync(_contrato.ContractNumber);
            if (!cuotas.Success || cuotas.Data == null)
            {
                ToastService.ShowError(cuotas.Message);
                return;
            }

            var pendientes = cuotas.Data.Where(c => c.SaldoPendiente > 0).ToList();
            if (pendientes.Count == 0)
            {
                ToastService.ShowInfo("Este contrato no tiene saldo pendiente.");
                return;
            }

            var dialog = new RegistrarPagoDialog(_contrato.ContractNumber, _contrato.CustomerName, pendientes)
            { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;

            BtnRegistrarPago.IsEnabled = false;
            try
            {
                var result = await _service.RegistrarPagoAsync(_contrato.ContractNumber, dialog.Monto, DateTime.Now);
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

                await RecargarAsync();
            }
            finally
            {
                BtnRegistrarPago.IsEnabled = true;
            }
        }

        // Después de un pago, el resumen y las dos pestañas se leen de nuevo de la base
        private async Task RecargarAsync()
        {
            var lista = await _service.ListCreditContractsDtoAsync();
            if (lista.Success && lista.Data != null)
            {
                var fresco = lista.Data.FirstOrDefault(c => c.ContractNumber == _contrato.ContractNumber);
                if (fresco != null)
                {
                    _contrato = fresco;
                    PintarResumen();
                }
            }

            await CargarCuotasAsync();
            await VistaRecibos.CargarAsync(_contrato.ContractNumber);
        }
    }
}