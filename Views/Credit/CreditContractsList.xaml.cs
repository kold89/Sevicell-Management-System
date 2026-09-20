using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
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

        private async Task LoadContractsData()
        {
            var contractsList = await ContractsSevices.ListCreditContractsDtoAsync();
            DgContracts.ItemsSource = contractsList.Data;
        }

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

            // Reusamos la lista ya cargada en el grid en vez de volver a golpear la BD
            var contractSelection = (DgContracts.ItemsSource as IEnumerable<creditContractsDTO>)
                ?.FirstOrDefault(c => c.ContractNumber == contractId);

            if (contractSelection == null)
            {
                MessageBox.Show("No se encontró el contrato.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var cuotasResult = await ContractsSevices.GetDebtInstalmentAsync(contractId);
            if (!cuotasResult.Success || cuotasResult.Data == null || !cuotasResult.Data.Any())
            {
                MessageBox.Show("No se encontraron cuotas para este contrato.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cuotas = cuotasResult.Data;
            var primeraCuota = cuotas.OrderBy(c => c.DueDate).First();
            var ultimaCuota = cuotas.OrderBy(c => c.DueDate).Last();

            var dto = new TemplateContractDto
            {
                NombreVendedor = contractSelection.SellerName,
                DniVendedor = contractSelection.DniSeller,
                empresa = contractSelection.empresa,

                NombreComprador = contractSelection.CustomerName,
                DniComprador = contractSelection.DniCustomer,
                DomicilioComprador = contractSelection.CustomerAddress,

                Articulo = contractSelection.ProductName,
                Marca = contractSelection.Brand,
                Modelo = contractSelection.model,
                Color = contractSelection.colour,
                Imei = contractSelection.Imei,
                Imei2 = contractSelection.Imei2,

                PrecioTotal = contractSelection.PriceSales,
                Prima = contractSelection.DownPayment,
                CantidadCuotas = cuotas.Count(),
                ValorCuota = primeraCuota.ExpectedAmount,
                FrecuenciaPago = "Mensual",

                FechaInicio = primeraCuota.DueDate,
                FechaFin = ultimaCuota.DueDate,
                FechaFirma = DateTime.Now,
                Municipio = "Teupasenti",
                Departamento = "El Paraíso"
            };

            ContractsSevices.GenerarContratoDocumento(dto);
        }
    }
}