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
    /// Lógica de interacción para RepairManagementPage.xaml
    /// </summary>
    public partial class RepairManagementPage : Page
    {
        public RepairManagementPage()
        {
            InitializeComponent();
        }

        private void BtnRepair_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new WpfApp1.Views.RepairListPage());
        }
    }
}
