using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Sales
{
    /// <summary>
    /// Lógica de interacción para SalesInvoicePage.xaml
    /// </summary>
    public partial class SalesInvoicePage : Page
    {
        public class DetalleVenta
        {
            public int ProductId { get; set; }
            public string Code { get; set; }
            public string ProductNameVisual { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalItem => Quantity * UnitPrice;
        }

        private readonly ProductsServices _productServices = new ProductsServices();
        private readonly CustomerServices _customerServices = new CustomerServices();       
        private readonly PaymentMethodServices _paymentMethodServices = new PaymentMethodServices(); 
        private readonly SalesInvoiceServices _salesInvoiceServices = new();
        private bool _guardando = false;

        private List<ProductsDto> _productos = new();
        private ProductsDto? _productoSeleccionado;
        private decimal invoiceTotal;
        private DateTime _fechaVenta = DateTime.Today;
        private bool _seleccionandoDesdeLista = false;

        private ObservableCollection<DetalleVenta> _detalleVenta = new();
        private bool _paginaLista = false;

        public SalesInvoicePage()
        {
            InitializeComponent();
            InicializarFecha();
            LoadCustomers();
            LoadPaymentMethods();
            DgSalesDetail.ItemsSource = _detalleVenta;
            LoadProducts();
            _paginaLista = true;
        }

        private async void LoadProducts()
        {
            try
            {
                var data = await _productServices.ListAllProducts();
                if (!data.Success)
                {
                    MessageBox.Show(data.Message);
                    return;
                }
                _productos = data.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        private async void LoadCustomers()
        {
            try
            {
                var data = await _customerServices.ListAllCustomersForGrid(); 
                if (!data.Success)
                {
                    MessageBox.Show(data.Message);
                    return;
                }

                List<custumerDTO> customers = new List<custumerDTO> { new custumerDTO { Id = 0, Name = "Seleccione Cliente." } };
                customers.AddRange(data.Data); 
                
                CboCliente.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        private async void LoadPaymentMethods()
        {
            try
            {
                var data = await _paymentMethodServices.ListAllPaymentMethods();
                if (!data.Success)
                {
                    MessageBox.Show("Error al obtener los métodos de pago.");
                    return;
                }
                var paymentsMethod = new List<PaymentMethod> { new PaymentMethod { Id = 0, Name = "Seleccione..." } };
                paymentsMethod.AddRange(data.Data);


                CboMetodoPago.ItemsSource = paymentsMethod;
                CboMetodoPago.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        private void ChkClienteRegistrado_Checked(object sender, RoutedEventArgs e)
        {
            CboCliente.IsEnabled = true;
            CboCliente.SelectedValue = 0;

            TxtClienteMostrador.Visibility = Visibility.Collapsed;
        }

        private void ChkClienteRegistrado_Unchecked(object sender, RoutedEventArgs e)
        {
            CboCliente.IsEnabled = false;
            CboCliente.SelectedValue = null;
            TxtClienteMostrador.Visibility = Visibility.Visible;
        }

        private void TxtBuscarProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_seleccionandoDesdeLista) return;

            _productoSeleccionado = null;
            string texto = TxtBuscarProducto.Text.Trim();

            if (string.IsNullOrEmpty(texto))
            {
                PopupSugerencias.IsOpen = false;
                return;
            }

            var resultados = _productos
                .Where(p => p.name.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || (p.code != null && p.code.Contains(texto, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            LstSugerencias.ItemsSource = resultados;
            PopupSugerencias.IsOpen = resultados.Any();
        }

        private void SugerenciaItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ProductsDto producto)
            {
                _seleccionandoDesdeLista = true;
                _productoSeleccionado = producto;
                TxtBuscarProducto.Text = producto.name;
                TxtBuscarProducto.CaretIndex = TxtBuscarProducto.Text.Length;
                TxtPrice.Text = producto.salesPrice.ToString("N2"); 
                PopupSugerencias.IsOpen = false;
                _seleccionandoDesdeLista = false;

                e.Handled = true;
                TxtCant.Focus();
            }
        }

        public bool ValidateFields()
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("El producto es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TxtCant.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCant.Focus();
                return false;
            }
            if (!decimal.TryParse(TxtPrice.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a cero.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
                return false;
            }
            if (_productoSeleccionado.stock < cantidad) 
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {_productoSeleccionado.stock}", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCant.Focus();
                return false;
            }
            return true;
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateFields() && _productoSeleccionado != null)
                {
                    int cantidad = Convert.ToInt32(TxtCant.Text);

                   var existente = _detalleVenta.FirstOrDefault(x => x.ProductId == _productoSeleccionado.id);
                    if (existente != null)
                    {
                        if (_productoSeleccionado.stock < existente.Quantity + cantidad)
                        {
                            MessageBox.Show($"Stock insuficiente. Disponible: {_productoSeleccionado.stock}", "Validación",
                                            MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        existente.Quantity += cantidad;
                        DgSalesDetail.Items.Refresh();
                    }
                    else
                    {
                        var details = new DetalleVenta
                        {
                            ProductId = _productoSeleccionado.id,
                            Code = _productoSeleccionado.code,
                            ProductNameVisual = _productoSeleccionado.name,
                            UnitPrice = Convert.ToDecimal(TxtPrice.Text),
                            Quantity = cantidad
                        };
                        _detalleVenta.Add(details);
                    }

                    RecalcularTotales();

                    _productoSeleccionado = null;
                    TxtBuscarProducto.Clear();
                    TxtPrice.Text = "0.00";
                    TxtCant.Text = "1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el detalle de producto: " + ex.Message);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            if (boton?.DataContext is not DetalleVenta details)
                return;

            _detalleVenta.Remove(details);
            RecalcularTotales();
        }

        private void TxtDescuento_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalcularTotales();
        }

        private void RecalcularTotales()
        {
            if (!_paginaLista) return;

            decimal subtotal = _detalleVenta.Sum(x => x.TotalItem);
            decimal.TryParse(TxtDescuento.Text, out decimal descuento);

            if (descuento > subtotal) descuento = subtotal; 

            invoiceTotal = subtotal - descuento;

            txSubtotal.Text = subtotal.ToString("N2");
            txTotal.Text = invoiceTotal.ToString("N2");
        }

        private void InicializarFecha()
        {
            _fechaVenta = DateTime.Today;
            CalFechaVenta.SelectedDate = _fechaVenta;
            TxtFechaSeleccionada.Text = _fechaVenta.ToString("dd/MM/yyyy");
        }

        private void BorderFecha_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupCalendario.IsOpen = true;
        }

        private void CalFechaVenta_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalFechaVenta.SelectedDate.HasValue)
            {
                _fechaVenta = CalFechaVenta.SelectedDate.Value;
                TxtFechaSeleccionada.Text = _fechaVenta.ToString("dd/MM/yyyy");
                PopupCalendario.IsOpen = false;
            }
        }

        private bool ValidateInvoiceHeader()
        {
            if (_detalleVenta.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la venta.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ChkClienteRegistrado.IsChecked == true &&
                (CboCliente.SelectedValue == null || Convert.ToInt32(CboCliente.SelectedValue) == 0))
            {
                MessageBox.Show("Debe seleccionar un cliente registrado, o desmarcar la opción para venta de mostrador.",
                                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                CboCliente.Focus();
                return false;
            }

            if (CboMetodoPago.SelectedValue == null || Convert.ToInt32(CboMetodoPago.SelectedValue) == 0)
            {
                MessageBox.Show("Debe seleccionar un método de pago.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                CboMetodoPago.Focus();
                return false;
            }

            return true;
        }

        private async void BtnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (_guardando) return;
            if (!ValidateInvoiceHeader()) return;

            var confirmacion = MessageBox.Show(
                $"¿Confirma guardar la venta con {_detalleVenta.Count} producto(s) por un total de {invoiceTotal:N2}?",
                "Confirmar guardado", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                _guardando = true;
                BtnSaveInvoice.IsEnabled = false;
                BtnSaveInvoice.Content = "Guardando...";

                int? customerId = ChkClienteRegistrado.IsChecked == true
                    ? Convert.ToInt32(CboCliente.SelectedValue)
                    : null;

                int? paymentMethodId = CboMetodoPago.SelectedValue != null
                    ? Convert.ToInt32(CboMetodoPago.SelectedValue)
                    : null;

                decimal.TryParse(TxtDescuento.Text, out decimal descuento);

                var details = _detalleVenta
                    .Select(d => (d.ProductId, d.Quantity, d.UnitPrice))
                    .ToList();

                string invoiceNumber = $"V-{DateTime.Now:yyyyMMdd-HHmmss-fff}"; 

                var result = await _salesInvoiceServices.SaveInvoiceAsync(
                    invoiceNumber: invoiceNumber,
                    customerId: customerId,
                    paymentMethodId: paymentMethodId,
                    createdAt: _fechaVenta,
                    headerDiscount: descuento,
                    details: details);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //MessageBox.Show(result.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                ToastService.ShowSuccess(result.Message);
                ClearForm();

                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _guardando = false;
                BtnSaveInvoice.IsEnabled = true;
                BtnSaveInvoice.Content = "💾 Guardar Venta";
            }
        }

        private void ClearForm()
        {
            ChkClienteRegistrado.IsChecked = false;
            CboCliente.SelectedValue = null;
            CboMetodoPago.SelectedValue = 0;
            CalFechaVenta.SelectedDate = DateTime.Today;

            TxtBuscarProducto.Clear();
            TxtCant.Text = "1";
            TxtPrice.Text = "0.00";
            TxtDescuento.Text = "0.00";
            _productoSeleccionado = null;

            _detalleVenta.Clear();
            invoiceTotal = 0;
            txSubtotal.Text = "0.00";
            txTotal.Text = "0.00";
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿Está seguro que desea cancelar? Se perderán los datos ingresados.",
                "Confirmar cancelación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;
            ClearForm();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}
