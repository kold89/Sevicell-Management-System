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
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private readonly ProductsServices productsServices = new ProductsServices();
        public Products()
        {
            InitializeComponent();
            LoadData();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            DialogAddProducto modalAddProducts = new DialogAddProducto();
            modalAddProducts.Owner = Window.GetWindow(this);
            bool? result = modalAddProducts.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }
        /// <summary>
        /// Función que carga la lista de productos al Datagrid.
        /// </summary>
        private async void LoadData()
        {
            ServicesResult<List<ProductsDto>> productDto = await productsServices.listProducForGrid();
            if (productDto.Success)
            {
                dgProducts.ItemsSource = productDto.Data;
            }
            else
            {
                dgProducts.ItemsSource = null;
                MessageBox.Show(productDto.Message);
            }
        }

        private void BtnSee_Click(object sender, RoutedEventArgs e)
        {
            var selected = (ProductsDto)dgProducts.SelectedItem;
            var productSelected =  productsServices.SearchProductById(selected.id);

            DialogProductForSee modalSeeProducts = new DialogProductForSee(productSelected.Data);
            modalSeeProducts.Owner = Window.GetWindow(this);
            modalSeeProducts.ShowDialog();

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            DialogAddProducto modalEditProduc = new DialogAddProducto();
            modalEditProduc.Owner = Window.GetWindow(this);
            bool? resutl = modalEditProduc.ShowDialog();
            if (resutl == true) {
                LoadData();
            }
        }

        private void BtnDisable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
             if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
