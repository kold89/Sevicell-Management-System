using System.Text;
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
using WpfApp1.Security;
using WpfApp1.Services;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void btnIngresar(object sender, RoutedEventArgs e)
        {
            try
            {
                lblMensageError.Text = String.Empty;
                var user = txtUsuario.Text.Trim();

                var service = new UserServices();
                if (String.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    lblMensageError.Text = "El usuario es Obligatorio";
                    txtUsuario.Focus();
                }
                else if (String.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    lblMensageError.Text = "La contraseña es Obligatoria";
                    txtPassword.Focus();
                }
                else
                {
                    var pass = Security.Security.HashPassword(txtPassword.Password);
                    var loggedUser = service.ValidateUser(txtUsuario.Text, pass);
                    if (loggedUser != null)
                    {
                        SessionManager.loggedInUser = loggedUser;
                        //SessionManager.Login(loggedUser,);
                        main main = new main();

                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        lblMensageError.Text = "Usuario o contraseña incorrecta.";
                        txtUsuario.Text = string.Empty;
                        txtPassword.Password = string.Empty;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar con el servidor de Sevicell. " + ex.Message + "Error Critico");
            }         
        }

    }
}