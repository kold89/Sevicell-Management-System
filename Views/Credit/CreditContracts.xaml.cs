using System;
using System.Collections.Generic;
using System.Data;
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
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;
using WpfApp1.Views.Inventory;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para CreditContracts.xaml
    /// </summary>
    public partial class CreditContracts : Page
    {

        private readonly CreditContractsServices ContractsSevices = new CreditContractsServices();
        public CreditContracts()
        {
            InitializeComponent();
            LoadCBox();

            txtPrecioVenta.TextChanged += (s, e) => ActualizarResumen();
            txtCuotaInicial.TextChanged += (s, e) => ActualizarResumen();
            txtNumCuotas.TextChanged += (s, e) => ActualizarResumen();
            cmbFrecuencia.SelectionChanged += (s, e) => ActualizarResumen();
            txtTasa.TextChanged += (s, e) => ActualizarResumen();
            dpPrimerVencimiento.SelectedDateChanged += (s, e) => ActualizarResumen();
        }
        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditCustumer  modalCustomer = new AddOrEditCustumer();
            modalCustomer.Owner = Window.GetWindow(this);
            bool? result = modalCustomer.ShowDialog();

            if (result == true)
            {
                LoadCboxCustumer();
            }
        }
        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            btnGuardar.IsEnabled = false;
            try
            {
                decimal.TryParse(txtPrecioVenta.Text, out decimal priceSale);
                decimal.TryParse(txtCuotaInicial.Text, out decimal enganche);
                decimal.TryParse(txtTasa.Text, out decimal tasa);

                var dto = new ContractCreateDto
                {
                    ClientId = (int)cmbCliente.SelectedValue,
                    SellerId = (int)cmbVendedor.SelectedValue,
                    ProductUnitId = (int)cmbProducto.SelectedValue,
                    FrequencyId = (int)cmbFrecuencia.SelectedValue,
                    FrequencyCode = ((viewFrecuency)cmbFrecuencia.SelectedItem).name,
                    SalePrice = priceSale,
                    DownPayment = enganche,
                    InstallmentCount = int.Parse(txtNumCuotas.Text),
                    FirstDueDate = dpPrimerVencimiento.SelectedDate ?? DateTime.Today,
                    interes = Convert.ToInt32(tasa)
                    //Notes = txtNotas.Text
                };

                var validation = ContractsSevices.ValidateContract(dto);
                if (!validation.Success)
                {
                    ToastService.ShowError(validation.Message);
                    return;
                }

                var calc = new ContractCalculationService().Calculate(dto);

                var save = await ContractsSevices.SaveContractAsync(dto, calc);
                if (!save.Success)
                {
                    ToastService.ShowError(save.Message);
                    return;
                }
                ToastService.ShowSuccess("Contrato creado exitosamente.");

                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar los datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnGuardar.IsEnabled = true;
            }
        }

        private  void LoadCboxCustumer()
        {
            //Clientes
            var custumer = new List<viewCustumers> { new viewCustumers { id = 0, name = "--Seleccione--" } };
            var customerDb = ContractsSevices.ListCustomers();
            if (customerDb.Success) { custumer.AddRange(customerDb.Data); }

            cmbCliente.ItemsSource = custumer;
            cmbCliente.SelectedIndex = 0;
        }
        private void LoadCBox()
        {
            try
            {
                //Vendedores
                var seller = new List<ViewSeller> { new ViewSeller { id = 0, name = "--Seleccione--" } };
                var listSellers = ContractsSevices.ListSellers();
                if (listSellers.Success) { seller.AddRange(listSellers.Data); }
                cmbVendedor.ItemsSource = seller;
                cmbVendedor.SelectedIndex = 0;

                //clientes
                LoadCboxCustumer();

                //Frecuencias
                var frecuency = new List<viewFrecuency> { new viewFrecuency { id = 0, name = "--Seleccione.--" } };
                var frecuenciesDB = ContractsSevices.ListFrecuency();
                if (frecuenciesDB.Success) { frecuency.AddRange(frecuenciesDB.Data); }

                cmbFrecuencia.ItemsSource = frecuency;
                cmbFrecuencia.SelectedIndex = 0;

                //Productos
                var phones = new List<viewProductUnit> { new viewProductUnit { id = 0, name = "--Seleccione.--" } };
                var phonesDb = ContractsSevices.ListProductsUnit();
                if (phonesDb.Success) {  phones.AddRange(phonesDb.Data); }

                cmbProducto.ItemsSource = phones;
                cmbProducto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar las listas desplegables.: " + ex.Message);
            }
        }
        private void cmbVendedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVendedor.SelectedItem is ViewSeller vendedorSeleccionado && vendedorSeleccionado.id != 0)
            {
                txtInfoVendedor.Text = $"DNI/Identidad: {vendedorSeleccionado.dni ?? "—"}\n" +
                                       $"Tel: {vendedorSeleccionado.phone ?? "—"}\n" +
                                       $"Empres: {vendedorSeleccionado.company ?? "-"}";
            }
            else
            {
                txtInfoVendedor.Text = "DNI/Identidad: —\nTel: —";
            }
        }
        private void cmbCliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCliente.SelectedItem is viewCustumers clienteSeleccionado && clienteSeleccionado.id != 0)
            {
                txtInfoCliente.Text = $"DNI/Identidad: {clienteSeleccionado.dni ?? "—"}\n" +
                                      $"Tel: {clienteSeleccionado.phone ?? "—"}\n" +
                                      $"Dirección: {clienteSeleccionado.direction ?? "—"}";
            }
            else
            {
                txtInfoCliente.Text = "DNI/Identidad: —\nTel: —\nDirección: —";
            }
        }
        private void cmbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProducto.SelectedItem is viewProductUnit productSeleccionado && productSeleccionado.id != 0)
            {
                txtInfoPhone.Text = $"Nombre: {productSeleccionado.name ?? "—"}\n" +
                                      $"IMEI2: {productSeleccionado.IMEI2 ?? "—"}\n" +
                                      $"Precio al Contado: {productSeleccionado.priceSales}\n"+
                                      $"Descripción: {productSeleccionado.description ?? "—"}";

                txtPrecioVenta.Text = Convert.ToString(productSeleccionado.priceSales);
            }
            else
            {
                txtInfoCliente.Text = "Nombre: —\nIMEI: —\nDescripcción: —";
                txtPrecioVenta.Text = " ";

            }
        }
        private void ActualizarResumen()
        {
            try
            {
                if (!decimal.TryParse(txtPrecioVenta.Text, out decimal precio)) return;
                if (!decimal.TryParse(txtCuotaInicial.Text, out decimal enganche)) return;
                if (!int.TryParse(txtNumCuotas.Text, out int cuotas) || cuotas <= 0) return;
                if (cmbFrecuencia.SelectedValue == null || (int)cmbFrecuencia.SelectedValue == 0) return;
                if (dpPrimerVencimiento.SelectedDate == null) return;
                if (!decimal.TryParse(txtTasa.Text, out decimal montoInteres)) return;
                
                var dto = new ContractCreateDto
                {
                    SalePrice = precio,
                    DownPayment = enganche,
                    InstallmentCount = cuotas,
                    FrequencyCode = ((viewFrecuency)cmbFrecuencia.SelectedItem).name,
                    FirstDueDate = dpPrimerVencimiento.SelectedDate.Value,
                    interes = montoInteres
                };

                var calc = new ContractCalculationService().Calculate(dto);

                runResumenPrecio.Text = $"L. {precio:N2}";
                runResumenInicial.Text = $"L. {enganche:N2}";
                runResumenSaldo.Text = $"L. {calc.FinancedBalance:N2}";
                runResumenCuotas.Text = cuotas.ToString();
                runResumenVencimiento.Text = calc.Installments.Last().DueDate.ToString("dd/MM/yyyy");
                runResumenCuotaMonto.Text = $"L. {calc.InstallmentAmount:N2}";
            }
            catch
            {
            }
        }
    }
}
