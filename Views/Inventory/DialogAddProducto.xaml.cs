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
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para DialogAddProducto.xaml
    /// </summary>
    public partial class DialogAddProducto : Window
    {
        private readonly ProductsServices services = new ProductsServices();
        private readonly Product? _product;
        public DialogAddProducto(Product product = null )
        {
            InitializeComponent();
            this.PreviewKeyDown += DialogAddProducto_PreviewKeyDown;
            _product = product;

            LoadBrandsAndCategory();
            CkStatus.IsChecked = true;
            this.Loaded += DialogAddProducto_Loaded;

            if (_product != null)
            {
                FillEditModal();
                CboBrand.SelectedValue = _product.BrandId ?? 0;
                CboCategory.SelectedValue = _product.CategoryId ?? 0;
            }
        }
        private void DialogAddProducto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Si el técnico ya está escribiendo en el Nombre, Precio o Descripción, 
            // no queremos que la ventana interfiera con su teclado mecánico.
            if (TxtNombre.IsFocused || txtDescription.IsFocused || TxtPrecioVenta.IsFocused | TxtStockInicial.IsFocused | TxtStockMinimo.IsFocused)
            {
                return;
            }

            // Si llega el ENTER definitivo que envía el escáner al final
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(TxtCodigo.Text))
                {
                    // El código ya se llenó con el escáner, mandamos al técnico a escribir el nombre
                    TxtNombre.Focus();
                    e.Handled = true; // Frenamos el Enter para que no haga ruidos extraños
                }
                return;
            }

            // Convertimos la tecla presionada en un caracter de texto legible
            string teclaPresionada = ConvertKeyToChar(e.Key);

            // Si la tecla es un número (0-9), la metemos directo al TextBox del código
            if (System.Text.RegularExpressions.Regex.IsMatch(teclaPresionada, "[0-9]"))
            {
                TxtCodigo.Text += teclaPresionada;
                e.Handled = true; // Evitamos que Windows use la tecla para otra cosa
            }
        }
        private string ConvertKeyToChar(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9) return (key - Key.D0).ToString();
            if (key >= Key.NumPad0 && key <= Key.NumPad9) return (key - Key.NumPad0).ToString();
            return string.Empty;
        }
        private void DialogAddProducto_Loaded(object sender, RoutedEventArgs e)
        {
            TxtCodigo.Focus(); 
        }
        public void FillEditModal()
        {
            
            txtTitle.Text = "Editar Producto";
            TxtStockInicial.Visibility = Visibility.Collapsed;
            lbStockInitial.Visibility = Visibility.Collapsed;


            TxtCodigo.Text = _product.Code;
            TxtNombre.Text = _product.Name;
            TxtPrecioVenta.Text = Convert.ToString(_product.SalePrice);
            TxtStockMinimo.Text = Convert.ToString(_product.MinimumStock);
            txtDescription.Text = _product.ProductDescription;
            CkStatus.IsChecked = _product.Status == true ? true: false;
            BtnClear.Visibility = Visibility.Collapsed;
        }

        private void LoadBrandsAndCategory()
        {
            try
            {
                var data =  services.listCategoryAndBrand();
                if (data.Success)
                {
                    var (brand, category) = data.Data;

                    var brands = new List<Brand>();
                    brands.Add(new Brand { Id = 0, Name = "--Seleccione Marca.--" });
                    var categories = new List<Category>();
                    categories.Add(new Category { Id = 0, Name = "--Seleccione Categoría.--" });

                    brands.AddRange(brand);
                    categories.AddRange(category);

                    CboBrand.ItemsSource = brands;
                    CboCategory.ItemsSource = categories;
                    CboBrand.SelectedValue = 0;
                    CboCategory.SelectedValue = 0;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado " + ex.Message);
            }

        }
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnSave.IsEnabled = false;
                if (!ValidateFields())  return;                

                if (_product == null)
                {
                    var product = new Product();
                    product.Code = TxtCodigo.Text;
                    product.Name = TxtNombre.Text;
                    product.SalePrice = Convert.ToDecimal(TxtPrecioVenta.Text);
                    product.MinimumStock = Convert.ToInt32(TxtStockMinimo.Text);
                    product.Stock = Convert.ToInt32(TxtStockInicial.Text);
                    product.BrandId = (int)CboBrand.SelectedValue;
                    product.CategoryId = (int)CboCategory.SelectedValue;
                    product.ProductDescription = txtDescription.Text;
                    //product.CreatedAt = DateTime.Now;
                    product.Status = true;

                    var result = await services.RegisterProductAsync(product);

                    if (result.Success)
                        MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                    {
                        MessageBox.Show("Error al registrar el producto.","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    _product.Code = TxtCodigo.Text;
                    _product.Name = TxtNombre.Text;
                    _product.SalePrice = Convert.ToDecimal(TxtPrecioVenta.Text);
                    _product.MinimumStock = Convert.ToInt32(TxtStockMinimo.Text);
                    _product.BrandId = (int)CboBrand.SelectedValue;
                    _product.CategoryId = (int)CboCategory.SelectedValue;
                    _product.ProductDescription = txtDescription.Text;
                    _product.Status = CkStatus.IsChecked;

                    var update = await services.UpdateProductAsync(_product);

                    if (update.Success)
                        MessageBox.Show("producto editado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex) 
            {
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool ValidateFields()
        {
            // Validar Nombre
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MessageBox.Show("El Código es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCodigo.Focus();
                return false;
            }
            //Valida apellidos
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese Nombre del Producto.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNombre.Focus();
                return false;
            }
            //valida Usuario
            if (string.IsNullOrWhiteSpace(TxtPrecioVenta.Text))
            {
                MessageBox.Show("Por favor, ingrese precio de venta.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida contraseña
            if (_product is null && string.IsNullOrWhiteSpace(TxtStockInicial.Text))
            {
                MessageBox.Show("Por favor, ingrese el stock inicial.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }  
            if (CboCategory.SelectedValue == null || (int)CboCategory.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione un Categoria válida.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida Usuario
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Por favor, ingrese descripción.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (CboBrand.SelectedValue == null || (int)CboBrand.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione una marca válido.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void btngenerateCode_Click(object sender, RoutedEventArgs e)
        {
            // Generamos un código único. 
            // Opción A: Puedes usar la fecha/hora actual en milisegundos para que nunca se repita:
            string codigoProvisional = "INT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Opción B: Si prefieres algo más corto pero aleatorio (un número de 6 dígitos):
            // Random rand = new Random();
            // string codigoProvisional = "INT-" + rand.Next(100000, 999999).ToString();

            // Asignamos el código generado al TextBox
            TxtCodigo.Text = codigoProvisional;

            // Pasamos el foco automáticamente al siguiente campo (Nombre de Producto) 
            // para que el técnico no tenga que usar el mouse
            TxtNombre.Focus();
        }

        private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Si el técnico escanea algo físico y presiona Enter, 
                // saltamos de inmediato al campo del Nombre
                if (!string.IsNullOrWhiteSpace(TxtCodigo.Text))
                {
                    TxtNombre.Focus(); // Reemplaza por el x:Name real de tu textbox de Nombre
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtCodigo.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            TxtPrecioVenta.Text = string.Empty;
            txtDescription.Text = string.Empty; 
            TxtStockInicial.Text = string.Empty;
            TxtStockMinimo.Text = string.Empty;
            CboBrand.SelectedValue = 0;
            CboCategory.SelectedValue = 0;
        }
    }
}
