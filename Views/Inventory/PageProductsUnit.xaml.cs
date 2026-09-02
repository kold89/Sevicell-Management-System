using Microsoft.EntityFrameworkCore;
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
using WpfApp1.Models.Enums;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para PageProductsUnit.xaml
    /// </summary>
    public partial class PageProductsUnit : Page
    {
        public ObservableCollection<ProductUnit> Unidades { get; set; } = new();
        private readonly productUnitServices ProdcUnitServices = new();
        public PageProductsUnit()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var listProducts = await ProdcUnitServices.GetProductsSerialized();
            if(listProducts.Success && listProducts.Data.Count > 0)
            CboProducto.ItemsSource = listProducts.Data;
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private async void CboProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new SevicellDbContext())
            {
                if (CboProducto.SelectedValue is not int productId)
                {
                    PnlForm.Visibility = Visibility.Collapsed;
                    Unidades.Clear();
                    return;
                }

                PnlForm.Visibility = Visibility.Visible;
                await LoadUnidades(productId);
            }
        }
        private async Task LoadUnidades(int productId)
        {
            using (var db = new SevicellDbContext())
            {
                var lista = await db.ProductUnits
                    .Where(u => u.ProductId == productId)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                Unidades.Clear();
                foreach (var u in lista) Unidades.Add(u);

                TxtDisponibles.Text = Unidades.Count(u => u.Status == "Disponible").ToString();
            }
        }
        private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var imei1 = TxtImei1.Text?.Trim();
            if (string.IsNullOrWhiteSpace(imei1))
            {
                MessageBox.Show("Ingresa al menos el IMEI 1.");
                return;
            }

            if (Unidades.Any(u => u.Imei == imei1))
            {
                MessageBox.Show("Ese IMEI ya está registrado.");
                return;
            }

            var productId = (int)CboProducto.SelectedValue;

            using (var db = new SevicellDbContext())
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var producto = await db.Products.FindAsync(productId);
                    if (producto == null)
                    {
                        ToastService.ShowInfo("El producto ya no existe.");
                        return; // no hay nada que revertir, no se abrió ningún SaveChanges todavía
                    }

                    var nueva = new ProductUnit
                    {
                        ProductId = productId,
                        Imei = imei1,
                        Imei2 = TxtImei2.Text?.Trim(),
                        Colour = TxtColor.Text?.Trim(),
                        Model = TxtModelo.Text?.Trim(),
                        Status = EProductUnitStatus.Disponible.ToDbValue(),
                        CreatedAt = DateTime.Now
                    };

                    db.ProductUnits.Add(nueva);
                    producto.Stock = (producto.Stock ?? 0) + 1;

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Solo tocamos la UI si la transacción se confirmó
                    Unidades.Insert(0, nueva);
                    TxtImei1.Clear(); TxtImei2.Clear(); TxtColor.Clear(); TxtModelo.Clear();
                    TxtImei1.Focus();

                    TxtDisponibles.Text = Unidades.Count(u => u.Status == "Disponible").ToString();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ToastService.ShowError($"No se pudo guardar la unidad. No se aplicó ningún cambio.\n\n{ex.Message}");
                }
            }
        }
        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var botom = sender as Button;
            if (botom?.DataContext is not ProductUnit productsDto)
                return;

            var productSelected = ProdcUnitServices.SearchProductUnitById(productsDto.Id);
            if (productSelected.Data == null || !productSelected.Success)
                return;

            DialogEditProductUnit modalEdit = new DialogEditProductUnit(productSelected.Data);
            modalEdit.Owner = Window.GetWindow(this);
            bool? result = modalEdit.ShowDialog();

            if (result == true && CboProducto.SelectedValue is int productId)
            {
                await LoadUnidades(productId);
            }

        }
    }
}
