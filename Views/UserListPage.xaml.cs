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
using WpfApp1.Models;
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
            modalUser.Owner = Window.GetWindow(this);
            bool? result = modalUser.ShowDialog();

            if (result == true) 
            { 
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
           var userSelect = (ViewUserDto)dgUsers.SelectedItem;

            if (userSelect != null)
            { 
                var userForEdit = serviceUse.SearchUser(userSelect.Id);
                var win = new DialogAddUser(userForEdit);
                win.Owner = Window.GetWindow(this);

                if (win.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private void BtnDisable_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = (ViewUserDto)dgUsers.SelectedItem;

            if (seleccionado != null)
            {
                switch(seleccionado.status)
                {
                    case "Activo":
                        var msjDisable = MessageBox.Show($"¿Está seguro que desea dar de baja a {seleccionado.Name}?",
                            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (msjDisable == MessageBoxResult.Yes)
                        {
                            serviceUse.DisableUser(seleccionado.Id);
                            LoadData();
                        }
                        break;

                    case "Deshabilitado":
                        var resultado = MessageBox.Show($"¿Está seguro que desea habilitar a {seleccionado.Name}?",
                            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (resultado == MessageBoxResult.Yes)
                        {
                            serviceUse.EnableUser(seleccionado.Id);
                            LoadData();
                        }
                        break;

                    default:
                        break;

                }
            
            }

        }
    }
}
