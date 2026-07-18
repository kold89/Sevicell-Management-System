using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para RoleEditWindow.xaml
    /// </summary>
    public partial class RoleEditWindow : Window
    {
        private roleServices servicesRole = new roleServices();
        private Role _rolExistente;
        public List<permissionSelected> listForUI {  get; set; }

        public RoleEditWindow(Role? rolParaEditar = null)
        {
            InitializeComponent();
            _rolExistente = rolParaEditar;
            LoadData();

            if (_rolExistente != null)
            {
                PrepararEdicion();
            }
        }
        private void PrepararEdicion()
        {
            txtNameRol.Text = _rolExistente.Name;
            txtDescriptionRol.Text = _rolExistente.Description;
            btnGuardar.Content = "ACTUALIZAR ROL";
            this.Title = "Editando Rol: " + _rolExistente.Name;

            // Obtener qué permisos tiene actualmente
            var idsAsignados = servicesRole.GetIdsPermisosPorRol(_rolExistente.Id);
            if (idsAsignados.Success)
            {
                // Marcarlos en la lista de la UI
                foreach (var p in listForUI)
                {
                    if (idsAsignados.Data.Contains(p.PermisoId))
                    {
                        p.ItemSelected = true;
                    }
                }
            }
        }
        public bool ValidateFilds()
        {
            // 1. Validar Nombre del Rol
            if (string.IsNullOrWhiteSpace(txtNameRol.Text))
            {
                MessageBox.Show("El nombre del rol es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNameRol.Focus(); // Pone el cursor ahí para ayudar al usuario
                return false;
            }

            // 2. Validar Descripción
            if (string.IsNullOrWhiteSpace(txtDescriptionRol.Text))
            {
                MessageBox.Show("Debes ingresar una descripción para el rol.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescriptionRol.Focus();
                return false;
            }

            // 3. Validar que al menos haya un permiso seleccionado
            // Esto evita crear roles "vacíos" que no sirven para nada
            bool tienePermisos = listForUI.Any(p => p.ItemSelected);

            if (!tienePermisos)
            {
                MessageBox.Show("Debes asignar al menos un permiso a este rol.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true; // Si pasó todo, regresamos true
        
        }
        private void LoadData()
        {
            // Traer de la DB
            var permissionsDB = servicesRole.GetPermissions();

            // Convertir a nuestra clase con Checkbox
            listForUI = permissionsDB.Select(x => new permissionSelected
            {
                PermisoId = x.Id,
                Name = x.Name,
                Code = x.Code,
                Modulo = x.Modulo,
                Description = x.Description,
                ItemSelected = false
            }).ToList();

            lvPermisos.ItemsSource = listForUI;

            // Lógica de agrupamiento por "Modulo"
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvPermisos.ItemsSource);
            if (view != null)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription("Modulo"));
            }
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkSelectAll.IsChecked ?? false;

            foreach (var p in listForUI)
            {
                p.ItemSelected = isChecked;
            }

            lvPermisos.Items.Refresh();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidateFilds()) return;

            ServicesResult<bool> result;

            var idsSeleccionados = listForUI
                .Where(p => p.ItemSelected)
                .Select(p => p.PermisoId)
                .ToList();
            bool exito = false;

            if (_rolExistente == null)
            {
                result = await servicesRole.RegistrarNuevoRolCompletoAsync(
                    txtNameRol.Text,
                    txtDescriptionRol.Text,
                    idsSeleccionados
                    );

            }
            else // MODO EDICIÓN
            {
                result = await servicesRole.ActualizarRolCompletoAsync(_rolExistente.Id, txtNameRol.Text, txtDescriptionRol.Text, idsSeleccionados);
            }

            if (result.Success)
            {
                MessageBox.Show("Datos guardados correctamente.", "SEVICELL");
                await PermissionManager.LoadPermissionAsync(_rolExistente.Id);

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error interno al procesar el rol.", "Error");
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
