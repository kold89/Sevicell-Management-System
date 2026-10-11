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
        private readonly creditContractsDTO _contrato;
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
    }
}