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
using WpfApp1.Data;
using WpfApp1.Security;

namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para main.xaml
    /// </summary>
    public partial class main : Window
    {
        public main()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            // 1. Mostrar el nombre del usuario logueado
            if (SessionManager.loggedInUser != null)
            {
                LblUserName.Text = SessionManager.loggedInUser.Name;
            }
        }

        private void btnConfiguraciones_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.ConfigDashboardPage();

            MainFrame.Navigate(confiCards);
        }
    }
}
