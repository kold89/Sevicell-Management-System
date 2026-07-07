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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para InvoicePage.xaml
    /// </summary>
    public partial class InvoicePage : Page
    {
        public class DetalleCompra
        {
            public int ProductId { get; set; }
            public string Code { get; set; }
            public string ProductNameVisual { get; set; }
            public int Quantity { get; set; }
            public decimal PurchasePrice { get; set; }
            public decimal TotalItem => Quantity * PurchasePrice; // calculado, no se guarda aparte
        }

        private readonly ProductsServices _productServices = new ProductsServices();
        private readonly SupplierServices _supplierServices = new SupplierServices();
        private readonly PurchaseInvoiceServices _purchaseInvoiceServices = new();
        private bool _guardando = false;

        private List<ProductsDto> _productos = new (); // tu catálogo completo
        private ProductsDto? _productoSeleccionado;
        private decimal invoiceTotal;
        private DateTime _fechaRegistro = DateTime.Today;

        private ObservableCollection<DetalleCompra> _detalleFactura = new();
        public InvoicePage()
        {
            InitializeComponent();
            InicializarFecha();
            LoadBrandsAndCategory();
            DgPurchaseDetail.ItemsSource = _detalleFactura; 
            LoadProducts();
        }
        private async void LoadProducts()
        {
            try
            {
                var data =  await _productServices.ListAllProducts();
                if (!data.Success)
                {
                    MessageBox.Show("Error al obtener la los productos.");
                    return ;
                }

                _productos = data.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado," + ex.Message);
            }
        }
        private void LstSugerencias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstSugerencias.SelectedItem is ProductsDto producto)
            {
                _productoSeleccionado = producto;
                TxtBuscarProducto.Text = producto.name;
                PopupSugerencias.IsOpen = false;
            }
        }

        private void TxtBuscarProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private async void LoadBrandsAndCategory()
        {
            try
            {
                var data = await _supplierServices.listSupplierForGrid();
                if (!data.Success)
                {
                    MessageBox.Show("Error inesperado " + data.Message);
                    return;
                }

                var supplier = new List<SupplierDto>();
                supplier.Add(new SupplierDto { id = 0, name = "--Seleccione Proveedor.--" });
                supplier.AddRange(data.Data);

                CboProveedor.ItemsSource = supplier;
                CboProveedor.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado " + ex.Message);
            }
        }
        private async void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            DialogAddProducto modalAddProducts = new DialogAddProducto();
            modalAddProducts.Owner = Window.GetWindow(this);
            bool? result = modalAddProducts.ShowDialog();

            LoadProducts();
        }
        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        public bool ValidateFields()
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("El producto es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCant.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtCant.Text))
            {
                MessageBox.Show("La cantidad es obligatoria.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCant.Focus();
                return false;
            }
            if (!int.TryParse(TxtCant.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCant.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtPrice.Text))
            {
                MessageBox.Show("El precio es obligatorio.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
                return false;
            }
            if (!decimal.TryParse(TxtPrice.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a cero.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
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
                    DetalleCompra details = new DetalleCompra();
                    details.ProductId = _productoSeleccionado.id;
                    details.Code = _productoSeleccionado.code;
                    details.ProductNameVisual = _productoSeleccionado.name;
                    details.PurchasePrice = Convert.ToDecimal(TxtPrice.Text);
                    details.Quantity = Convert.ToInt32(TxtCant.Text);
                    _detalleFactura.Add(details);
                    invoiceTotal = _detalleFactura.Sum(x => x.TotalItem);
                    txTotal.Text = invoiceTotal.ToString("N2");

                    _productoSeleccionado = null;
                    TxtBuscarProducto.Clear();
                    TxtPrice.Text = "0.00";
                    TxtCant.Text = "1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el detalle de producto.");
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var botom = sender as Button;
            if (botom?.DataContext is not DetalleCompra details)
                return;

            _detalleFactura.Remove(details);
            invoiceTotal = _detalleFactura.Sum(x => x.TotalItem);
            txTotal.Text = invoiceTotal.ToString("N2");
        }

        private void ChkInformal_Checked(object sender, RoutedEventArgs e)
        {
            TxtInvoiceNumber.Text = generateCodInf();
        }

        private void ChkInformal_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtInvoiceNumber.ClearValue(TextBox.TextProperty);
        }
        private string generateCodInf()
        {
            // Ejemplo: INF-20260706-143205-873
            return $"INF-{DateTime.Now:yyyyMMdd-HHmmss-fff}";
        }

        private void ClearForm()
        {
            ChkInformal.IsChecked = false;
            TxtInvoiceNumber.Text = string.Empty;
            CboProveedor.SelectedValue = 0;
            TxtSupplierCasual.Clear();
            CalFechaRegistro.SelectedDate = DateTime.Today;

            TxtBuscarProducto.Clear();
            TxtCant.Text = "1";
            TxtPrice.Text = "0.00";
            _productoSeleccionado = null;

            _detalleFactura.Clear();
            invoiceTotal = 0;
            txTotal.Text = "0.00";
        }

        private void BnCancel_Click(object sender, RoutedEventArgs e)
        {
            var confirmacion = MessageBox.Show(
        "¿Está seguro que desea cancelar? Se perderán los datos ingresados.",
        "Confirmar cancelación",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes)
                return;

            ClearForm();
        }

        private void InicializarFecha()
        {
            _fechaRegistro = DateTime.Today;
            CalFechaRegistro.SelectedDate = _fechaRegistro;
            TxtFechaSeleccionada.Text = _fechaRegistro.ToString("dd/MM/yyyy");
        }

        private void BorderFecha_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupCalendario.IsOpen = true;
        }

        private void CalFechaRegistro_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalFechaRegistro.SelectedDate.HasValue)
            {
                _fechaRegistro = CalFechaRegistro.SelectedDate.Value;
                TxtFechaSeleccionada.Text = _fechaRegistro.ToString("dd/MM/yyyy");
                PopupCalendario.IsOpen = false;
            }
        }

        private bool ValidateInvoiceHeader()
        {
            if (_detalleFactura.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la factura.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            bool esInformal = ChkInformal.IsChecked == true;

            if (esInformal)
            {
                if (string.IsNullOrWhiteSpace(TxtSupplierCasual.Text))
                {
                    MessageBox.Show("Debe indicar el nombre del vendedor casual.", "Validación",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtSupplierCasual.Focus();
                    return false;
                }
            }
            else
            {
                if (CboProveedor.SelectedValue == null || Convert.ToInt32(CboProveedor.SelectedValue) == 0)
                {
                    MessageBox.Show("Debe seleccionar un proveedor.", "Validación",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    CboProveedor.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TxtInvoiceNumber.Text))
                {
                    MessageBox.Show("Debe ingresar el número de factura fiscal.", "Validación",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtInvoiceNumber.Focus();
                    return false;
                }
            }

            return true;
        }

        private async void BtnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (_guardando) return;
            if (!ValidateInvoiceHeader()) return;

            var confirmacion = MessageBox.Show(
                $"¿Confirma guardar la factura con {_detalleFactura.Count} producto(s) por un total de {invoiceTotal:N2}?",
                "Confirmar guardado", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes) return;

            try
            {
                _guardando = true;
                BtnSaveInvoice.IsEnabled = false;
                BtnSaveInvoice.Content = "Guardando...";

                bool esInformal = ChkInformal.IsChecked == true;

                var details = _detalleFactura
                    .Select(d => (d.ProductId, d.Quantity, d.PurchasePrice))
                    .ToList();

                var result = await _purchaseInvoiceServices.SaveInvoiceAsync(
                    invoiceNumber: TxtInvoiceNumber.Text.Trim(),
                    invoiceNumberExist: !esInformal, 
                    supplierId: esInformal ? null : Convert.ToInt32(CboProveedor.SelectedValue),
                    supplierNameCasual: esInformal ? TxtSupplierCasual.Text.Trim() : null,
                    createdAt: _fechaRegistro,
                    details: details);

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show(result.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
                BtnSaveInvoice.Content = "💾 Guardar Factura";
            }
        }
    }
}
