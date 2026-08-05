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
        public readonly List<Tasa> tasas = new List<Tasa> {
                    { new Tasa{ id = 0, description = "--Seleccione--" } },
                    { new Tasa{ id = 1, description = "3%" } },
                    { new Tasa{ id = 2, description = "4%" } },
                    { new Tasa{ id = 3, description = "5%" } },
                    { new Tasa{ id = 4, description = "6%" } }
                };
        private readonly CreditContractsServices ContractsSevices = new CreditContractsServices();
        public CreditContracts()
        {
            InitializeComponent();
            LoadCBox();


            txtPrecioVenta.TextChanged += (s, e) => ActualizarResumen();
            txtCuotaInicial.TextChanged += (s, e) => ActualizarResumen();
            txtNumCuotas.TextChanged += (s, e) => ActualizarResumen();
            cmbFrecuencia.SelectionChanged += (s, e) => ActualizarResumen();
            cmbTasa.SelectionChanged += (s, e) => ActualizarResumen();
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
            decimal.TryParse(txtPrecioVenta.Text, out decimal priceSale);
            decimal.TryParse(txtCuotaInicial.Text, out decimal enganche);

            var dto = new ContractCreateDto
            {
                ClientId = (int)cmbCliente.SelectedValue,
                SellerId = (int)cmbVendedor.SelectedValue,
                ProductUnitId = (int)cmbProducto.SelectedValue,
                FrequencyId = (int)cmbFrecuencia.SelectedValue,
                FrequencyCode = ((viewFrecuency)cmbFrecuencia.SelectedItem).name,
                SalePrice = priceSale,
                //SalePrice = decimal.Parse(txtPrecioVenta.Text),
                //DownPayment = decimal.Parse(txtCuotaInicial.Text),
                DownPayment = enganche,
                InstallmentCount = int.Parse(txtNumCuotas.Text),
                FirstDueDate = dpPrimerVencimiento.SelectedDate ?? DateTime.Today,
                LateInterestRate = ParseTasa((int)cmbTasa.SelectedValue),
                //Notes = txtNotas.Text
            };

            var validation = ContractsSevices.ValidateContract(dto);
            if (!validation.Success)
            {
                //ToastService.ShowError(validation.Message);
               MessageBox.Show(validation.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var calc = new ContractCalculationService().Calculate(dto);

            var save = await ContractsSevices.SaveContractAsync(dto, calc);
            if (!save.Success)
            {
                //ToastService.ShowError(save.Message);
                MessageBox.Show("Error al querer guardar contrato.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Contrato creado exitosamente..", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
            //ToastService.ShowSuccess("Contrato creado exitosamente.");
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }

        private decimal ParseTasa(int tasaId) => tasaId switch
        {
            1 => 0.03m,
            2 => 0.04m,
            3 => 0.05m,
            4 => 0.06m,
            _ => 0m
        };

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
                //Carga de tasas
                cmbTasa.ItemsSource = tasas;
                cmbTasa.SelectedValue = 0;

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
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private void cmbVendedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbVendedor.SelectedItem is ViewSeller vendedorSeleccionado && vendedorSeleccionado.id != 0)
            {
                // Ajusta las propiedades de acuerdo a como están nombradas en tu clase ViewSeller
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
                // Si falta algo esencial para calcular, no revientes, solo salí
                if (!decimal.TryParse(txtPrecioVenta.Text, out decimal precio)) return;
                if (!decimal.TryParse(txtCuotaInicial.Text, out decimal enganche)) return;
                if (!int.TryParse(txtNumCuotas.Text, out int cuotas) || cuotas <= 0) return;
                if (cmbFrecuencia.SelectedValue == null || (int)cmbFrecuencia.SelectedValue == 0) return;
                if (dpPrimerVencimiento.SelectedDate == null) return;
                if (cmbTasa.SelectedValue == null || (int)cmbTasa.SelectedValue == 0) return;

                var dto = new ContractCreateDto
                {
                    SalePrice = precio,
                    DownPayment = enganche,
                    InstallmentCount = cuotas,
                    FrequencyCode = ((viewFrecuency)cmbFrecuencia.SelectedItem).name,
                    FirstDueDate = dpPrimerVencimiento.SelectedDate.Value,
                    LateInterestRate = ParseTasa((int)cmbTasa.SelectedValue)
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
                // datos incompletos o inválidos mientras el vendedor escribe -- no mostramos nada roto
            }
        }
    }
}
