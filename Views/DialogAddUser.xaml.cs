using DocumentFormat.OpenXml.Spreadsheet;
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
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para DialogAddUser.xaml
    /// </summary>
    public partial class DialogAddUser : Window
    {
        private readonly roleServices serviceRole = new roleServices();
        private readonly UserServices serviceUser = new UserServices();
        private User? userExist;
        private string passwordOrigin;

        public DialogAddUser(User? userForEdit = null)
        {
            InitializeComponent();
            userExist = userForEdit;
            LoadRoles();

            if (userExist != null) 
            {
                txtNombre.Text = userExist.Name;
                txtLasName.Text = userExist.LastName;
                txtLogin.Text = userExist.Username;
                txtPass.Password = userExist.Password;
                cbRoles.SelectedValue = userExist.RoleId;

                this.Title = "Actualizar Usuario";
                btnGuardar.Content = "Actualizar";

                passwordOrigin = userExist.Password;
            }
        }
        private void LoadRoles()
        {
            try {
                var rolesDb =  serviceRole.GetListRoles();
                if (!rolesDb.Success)
                {
                    MessageBox.Show(rolesDb.Message);
                    return;
                }

                var listadPlaceHolder = new List<Role>();
                listadPlaceHolder.Add(new Role { Id = 0, Name = "--Selecciones un Rol--" });

                listadPlaceHolder.AddRange(rolesDb.Data);
                cbRoles.ItemsSource = listadPlaceHolder;
                cbRoles.SelectedIndex = 0;
            } catch (Exception ex)
            {
                MessageBox.Show("Error inesperado " + ex.Message);
            }
            
        }
        public bool ValidateFields()
        {
            // Validar Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombre.Focus(); 
                return false;
            }
            //Valida apellidos
            if (string.IsNullOrWhiteSpace(txtLasName.Text))
            {
                MessageBox.Show("Por favor, ingrese Apellidos.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLasName.Focus(); 
                return  false;
            }
            //valida Usuario
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Por favor, ingrese un usuario.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida contraseña
            if (string.IsNullOrWhiteSpace(txtPass.Password))
            {
                MessageBox.Show("Por favor, ingrese una contraseña.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (cbRoles.SelectedValue == null || (int)cbRoles.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol válido para el usuario.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return  false;
            }

            return true;
        }
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (!ValidateFields()) return;

                bool isPassDiferent = false;

                if (userExist == null)
                {
                    var newUser = new User();
                    newUser.Name = txtNombre.Text;
                    newUser.LastName = txtLasName.Text;
                    newUser.Username = txtLogin.Text;
                    newUser.Password = txtPass.Password;
                    newUser.RoleId = (int)cbRoles.SelectedValue;
                    newUser.CreatedAt = DateTime.Now;
                    newUser.Status = true;

                    var user = await serviceUser.RegisterUserAsync(newUser);

                    if (user.Success)
                        ToastService.ShowSuccess(user.Message);
                }
                else
                {
                    isPassDiferent = (txtPass.Password != passwordOrigin);
                    if (isPassDiferent) userExist.Password = txtPass.Password;

                    userExist.Name = txtNombre.Text;
                    userExist.LastName = txtLasName.Text;
                    userExist.Username = txtLogin.Text;
                    userExist.RoleId = (int)cbRoles.SelectedValue;
                    userExist.UpdatedAt = DateTime.Now;

                    var update = await serviceUser.UpdateUserAsync(userExist, isPassDiferent);

                    if (update.Success)
                        ToastService.ShowSuccess(update.Message);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error inesperado "+ ex.Message);
            }
        }
    }
}
