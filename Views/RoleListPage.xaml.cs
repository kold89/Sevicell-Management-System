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
    /// Lógica de interacción para RoleListPage.xaml
    /// </summary>
    public partial class RoleListPage : Page
    {
        private readonly roleServices servicesRole = new roleServices();

        public RoleListPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        public void LoadData()
        {
            try
            {
                dgRoles.ItemsSource = servicesRole.GetRoleForDGrid();
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error al cargar los roles: " + ex.Message);
            }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            var modal = new RoleEditWindow { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true)
                LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: ViewRoleDto row }) return;

            var result = servicesRole.GetRolForId(row.id);
            if (!result.Success || result.Data == null)
            {
                ToastService.ShowError(result.Message ?? "No se pudo cargar el rol.");
                return;
            }

            var win = new RoleEditWindow(result.Data) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
                LoadData();
        }

        private async void BtnDisable_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: ViewRoleDto row }) return;

            bool estaActivo = row.status == "Activo";
            string accion = estaActivo ? "dar de baja" : "habilitar";

            var confirm = MessageBox.Show(
                $"¿Está seguro que desea {accion} el rol {row.name}?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var result = await servicesRole.ChangeRoleStatusAsync(row.id, !estaActivo);

                if (result.Success)
                {
                    LoadData();
                    ToastService.ShowSuccess(result.Message);
                }
                else
                {
                    ToastService.ShowError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error al cambiar el estado del rol: " + ex.Message);
            }
        }
    }
}
