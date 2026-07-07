using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            // 1. Definimos cuántos registros queremos por página (ejemplo: 15)
            MiPaginador.RegistrosPorPagina = 10;
            LoadData();
        }

        private async void LoadData()
        {
            // Nota: Lo ideal a futuro es que tu service acepte parámetros de paginación,
            // por ejemplo: productsServices.listProducForGrid(MiPaginador.PaginaActual, MiPaginador.RegistrosPorPagina);

            ServicesResult<List<ProductsDto>> productDto = await productsServices.listProducForGrid();

            if (productDto.Success && productDto.Data != null)
            {
                int totalRegistros = productDto.Data.Count;
                int cantidadPorPagina = MiPaginador.RegistrosPorPagina;

                // 3. Calculamos matemáticamente el total de páginas necesarias
                int totalPaginas = (int)Math.Ceiling((double)totalRegistros / cantidadPorPagina);
                MiPaginador.TotalPaginas = totalPaginas < 1 ? 1 : totalPaginas;

                // 4. Filtramos la lista completa usando LINQ (.Skip y .Take) para mostrar solo el segmento actual
                var datosPaginados = productDto.Data
                    .Skip((MiPaginador.PaginaActual - 1) * cantidadPorPagina)
                    .Take(cantidadPorPagina)
                    .ToList();

                dgProducts.ItemsSource = datosPaginados;
            }
            else
            {
                dgProducts.ItemsSource = null;
                MessageBox.Show(productDto?.Message ?? "Error al cargar los productos.");
            }
        }

        // Este evento se dispara sólito cada vez que el usuario presione una flecha
        private void MiPaginador_PaginaCambiada(object sender, EventArgs e)
        {
            // Al cambiar de página, simplemente volvemos a invocar a LoadData.
            // Como LoadData ahora lee "MiPaginador.PaginaActual", filtrará el DataGrid automáticamente.
            LoadData();
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
            var selectedItem = (ProductsDto)dgProducts.SelectedItem;
            var productSelected = productsServices.GetProductById(selectedItem.id);

            DialogAddProducto modalEditProduc = new DialogAddProducto(productSelected);
            modalEditProduc.Owner = Window.GetWindow(this);
            bool? resutl = modalEditProduc.ShowDialog();
            if (resutl == true) {
                LoadData();
            }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
             if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            DialogAddProducto modalAddProducts = new DialogAddProducto();
            modalAddProducts.Owner = Window.GetWindow(this);
            bool? result = modalAddProducts.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }
    }
}
