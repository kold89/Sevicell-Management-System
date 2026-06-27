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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para suppliersPage.xaml
    /// </summary>
    public partial class suppliersPage : Page
    {
        private readonly SupplierServices supplierServices = new SupplierServices();
        public suppliersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            ServicesResult<List<SupplierDto>> productDto = await supplierServices.listSupplierForGrid();

            if (productDto.Success && productDto.Data != null)
            {
                dgSupplier.ItemsSource = productDto.Data;
            }
            else
            {
                dgSupplier.ItemsSource = null;
                MessageBox.Show(productDto?.Message ?? "Error al cargar los proveedores.");
            }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditSupplier modalSupplier = new AddOrEditSupplier();
            modalSupplier.Owner = Window.GetWindow(this);
            bool? result = modalSupplier.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (SupplierDto)dgSupplier.SelectedItem;
            var supplierSelected = supplierServices.GetSupplierById(selectedItem.id);

            AddOrEditSupplier modalEdit = new AddOrEditSupplier(supplierSelected.Data);
            modalEdit.Owner = Window.GetWindow(this);
            bool? resutl = modalEdit.ShowDialog();
            if (resutl == true)
            {
                LoadData();
            }
        }

        private void BtnSee_Click(object sender, RoutedEventArgs e)
        {
            var selected = (SupplierDto)dgSupplier.SelectedItem;
            var supplierSelected = supplierServices.SearchSupplierById(selected.id);

            DialogSupplierForSee modalSee = new DialogSupplierForSee(supplierSelected.Data);
            modalSee.Owner = Window.GetWindow(this);
            modalSee.ShowDialog();
        }    
    }
}
