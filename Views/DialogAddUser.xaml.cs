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
            loadRoles();

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

        private void loadRoles()
        {
            
            var rolesDb = serviceRole.GetRoles();
            var listadPlaceHolder = new List<Role>();
            listadPlaceHolder.Add(new Role { Id = 0, Name = "--Selecciones un Rol--" });

            listadPlaceHolder.AddRange(rolesDb);
            cbRoles.ItemsSource = listadPlaceHolder;
            cbRoles.SelectedIndex = 0;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cbRoles.SelectedValue == null || (int)cbRoles.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol válido para el usuario.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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

                serviceUser.RegisterUser(newUser);
                MessageBox.Show("Usuario registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
        
                serviceUser.UpdateUser(userExist, isPassDiferent);
                MessageBox.Show("Usuario Editado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
