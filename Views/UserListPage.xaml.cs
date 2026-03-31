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
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para UserListPage.xaml
    /// </summary>
    public partial class UserListPage : Page
    {
        private DBSevicellContext db = new DBSevicellContext();
        private readonly UserServices serviceUse = new UserServices();
        public UserListPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            dgUsers.ItemsSource = serviceUse.GetUserForDGrid();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }

        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            DialogAddUser modalUser = new  DialogAddUser();
            modalUser.ShowDialog();
        }
    }
}
