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
    /// Lógica de interacción para RoleListPage.xaml
    /// </summary>
    public partial class RoleListPage : Page
    {
        private DBSevicellContext db = new DBSevicellContext();
        private readonly roleServices servicesRole = new roleServices();

        public RoleListPage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            dgRoles.ItemsSource = servicesRole.GetRoleForDGrid();
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
            RoleEditWindow modalRol = new RoleEditWindow();
            modalRol.Owner = Window.GetWindow(this);
            bool? result = modalRol.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private async void BtnDisable_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = (ViewRoleDto)dgRoles.SelectedItem;

            if (seleccionado != null)
            {
                bool newStatus = seleccionado.status == "Activo" ? false : true;
                string actionStatus = seleccionado.status == "Activo" ? "dar de baja" : "habilitar";

                var msjDisable = MessageBox.Show($"¿Está seguro que desea {actionStatus} el rol {seleccionado.name}?",
                            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msjDisable == MessageBoxResult.Yes)
                {
                    var result = await servicesRole.ChangeRoleStatusAsync(seleccionado.id, newStatus);

                    if (result.Success)
                    {
                        LoadData();
                        MessageBox.Show(result.Message);
                    }
                }
            }
        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            RoleEditWindow modalRol = new RoleEditWindow();
            modalRol.Owner = Window.GetWindow(this);
            bool? result = modalRol.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }
    }
}
