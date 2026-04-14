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
    /// Lógica de interacción para RepairListPage.xaml
    /// </summary>
    public partial class RepairListPage : Page
    {
        public RepairListPage()
        {
            InitializeComponent();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void dgRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnAddRepair_Click(object sender, RoutedEventArgs e)
        {
            DialogAddRepair modalRepairs = new DialogAddRepair();
            modalRepairs.Owner = Window.GetWindow(this);
            bool? result = modalRepairs.ShowDialog();
        }
    }
}
