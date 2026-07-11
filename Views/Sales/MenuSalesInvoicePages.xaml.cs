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

namespace WpfApp1.Views.Sales
{
    /// <summary>
    /// Lógica de interacción para MenuSalesInvoicePages.xaml
    /// </summary>
    public partial class MenuSalesInvoicePages : Page
    {
        public MenuSalesInvoicePages()
        {
            InitializeComponent();
        }

        private void btnSells_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Sales.SalesInvoicePage());
        }

        private void BtnHistorySells_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Sales.SalesInovoiceListPage());
        }

        private void BtnSellsDaily_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.Sales.SalesInvoiceDetails());
        }
    }
}
