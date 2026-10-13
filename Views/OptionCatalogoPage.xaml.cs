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

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para OptionCatalogoPage.xaml
    /// </summary>
    public partial class OptionCatalogoPage : Page
    {
        public OptionCatalogoPage()
        {
            InitializeComponent();
        }

        private void btnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Inventory.suppliersPage());
        }

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Inventory.BrandAndCategory());
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Catalog.CustomersListPage());
        }
    }
}
