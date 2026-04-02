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
using WpfApp1.Data;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para RoleListPage.xaml
    /// </summary>
    public partial class RoleListPage : Page
    {
        private DBSevicellContext db = new DBSevicellContext();
        private readonly roleServices serviceUse = new roleServices();

        public RoleListPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            dgRoles.ItemsSource = serviceUse.GetRoleForDGrid();
        }
        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDisable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            //DialogAddUser modalUser = new DialogAddUser();
            //modalUser.Owner = Window.GetWindow(this);
            //bool? result = modalUser.ShowDialog();

            //if (result == true)
            //{
            //    LoadData();
            //}
        }
    }
}
