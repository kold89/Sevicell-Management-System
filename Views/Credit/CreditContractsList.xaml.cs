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
    /// Lógica de interacción para CreditContractsList.xaml
    /// </summary>
    public partial class CreditContractsList : Page
    {
        private readonly CreditContractsServices ContractsSevices = new CreditContractsServices();

        public CreditContractsList()
        {
            InitializeComponent();
            LoadContractsData();
        }

        private void BtnAddContract_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditContracts());
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
  
        private void LoadContractsData()
        {
            var contractsList = ContractsSevices.ListCreditContractsDto();
          
            DgContracts.ItemsSource = contractsList.Data;
        }

        private async void DgContracts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgContracts.SelectedItem is not creditContractsDTO contractSelection)
                return;
            try
            {
                var result = await ContractsSevices.GetDebtInstalmentAsync(contractSelection.ContractNumber);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DgCuotas.ItemsSource = result.Data;

                var pagadas = result.Data.Count(x => x.Status == "PAID" || x.Status == "PAID_LATE");
                var totalPagado = result.Data.Where(x => x.Status == "PAID" || x.Status == "PAID_LATE").Sum(x => x.ExpectedAmount);

                var details = new ContractDetailViewModel
                {
                    ContractNumber =$"Contrato No. {Convert.ToString(contractSelection.ContractNumber)}",
                    CreatedAtText = contractSelection.CreatedAt.ToString("dd/MM/yyyy"),
                    ClientName = contractSelection.CustomerName,
                    ProductName = contractSelection.ProductName,
                    SalePrice = contractSelection.PriceSales, 
                    DownPayment = contractSelection.DownPayment,
                    PendingBalance = contractSelection.Balance,
                    TotalInstallments = result.Data.Count,
                    PaidInstallments = pagadas,
                    PendingInstallmentsCount = result.Data.Count - pagadas,
                    TotalPaid = totalPagado
                };

                PanelDetalle.DataContext = details;

                TxtSinSeleccion.Visibility = Visibility.Collapsed;
                PanelDetalle.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los pagos: " + ex.Message);
            }
        }
    }
}
