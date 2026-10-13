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
using WpfApp1.Views.Credit;

namespace WpfApp1.Views.Catalog
{
    /// <summary>
    /// Lógica de interacción para CustomersListPage.xaml
    /// </summary>
    public partial class CustomersListPage : Page
    {
        private readonly CustomerServices ContractsSevices = new CustomerServices();

        public CustomersListPage()
        {
            InitializeComponent();
            LoadCustomers();
        }
        private async void LoadCustomers()
        {
            try
            {
                var data = await ContractsSevices.ListAllCustomersForGrid();
                if (!data.Success)
                {
                    ToastService.ShowInfo(data.Message);
                    return;
                }
                dgCustomers.ItemsSource = data.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }
        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnAddCustomers_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditCustumer modalCustomer = new AddOrEditCustumer();
            modalCustomer.Owner = Window.GetWindow(this);
            bool? result = modalCustomer.ShowDialog();

            if (result == true)
            {
                LoadCustomers();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
