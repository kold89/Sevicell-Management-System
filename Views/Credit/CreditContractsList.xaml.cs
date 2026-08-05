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

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para CreditContractsList.xaml
    /// </summary>
    public partial class CreditContractsList : Page
    {
        public CreditContractsList()
        {
            InitializeComponent();
            LoadContractsData();
        }

        private void BtnAddContract_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditContracts());
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
        public class ContractViewModel
        {
            public string ContractNumber { get; set; }
            public string ProductName { get; set; }
            public string CustomerName { get; set; }
            public string Status { get; set; }

            public DateTime CreatedAt { get; set; }
            public decimal Balance { get; set; }
        }
        private void LoadContractsData()
        {
            // Simulación de datos (sustituir por consulta a base de datos o servicio)

            List<ContractViewModel> contractsList = new List<ContractViewModel>
            {
                new ContractViewModel 
                { 
                    ContractNumber = "CTR-001", 
                    ProductName = "Laptop Dell", 
                    CustomerName = "Juan Pérez", 
                    Status = "Activo", 
                    CreatedAt = DateTime.Now.AddDays(30), 
                    Balance = 1250.50m 
                },
                new ContractViewModel 
                { 
                    ContractNumber = "CTR-002", 
                    ProductName = "Smart TV 55\"", 
                    CustomerName = "María López", 
                    Status = "Cancelado", 
                    CreatedAt = DateTime.Now.AddDays(15), 
                    Balance = 0.00m 
                },
                new ContractViewModel 
                { 
                    ContractNumber = "CTR-003", 
                    ProductName = "Consola PS5", 
                    CustomerName = "Carlos Gómez", 
                    Status = "Deshabilitado", 
                    CreatedAt = DateTime.Now.AddDays(5), 
                    Balance = 450.00m 
                }
            };

            // Asignación de la lista al DataGrid por su x:Name
            DgContracts.ItemsSource = contractsList;
        }
    }
}
