using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;

namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para main.xaml
    /// </summary>
    public partial class main : Window
    {
        public record NotificationItem
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string TimeAgo { get; set; }
            public DateTime Date { get; set; }
            public bool IsRead { get; set; }
        }
        public main()
        {
            InitializeComponent();
            InitializeNotifications();
            AppPermissionsMenu();

            PermissionManager.PermisosActualizados += AppPermissionsMenu;
            this.WindowState = WindowState.Maximized;

            SessionManager.UserSessionChanged += UpdateUserSession;
            UpdateUserSession();
        }
        private void UpdateUserSession()
        {
            var user = SessionManager.loggedInUser;

            if (user != null)
            {
                string nombre = user.Name ?? "";
                string inicialApellido = !string.IsNullOrWhiteSpace(user.LastName)
                    ? $" {user.LastName.Trim()[0]}."
                    : "";

                LblUserName.Text = $"{nombre}{inicialApellido}";

                int? idRole = user.RoleId;
                LblUserRole.Text = SessionManager.GetProfile(idRole) ?? "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Marca el botón de Inicio (esto disparará BtnDashboard_Checked)
            BtnDashboard.IsChecked = true;
        }

        private void AppPermissionsMenu()
        {
            var MapsModules = new Dictionary<RadioButton, string>
            {
                { BtnSales, "MODULE_SALES_VIEW" },
                { BtnCredits, "MODULE_CREDITS_VIEW" },
                { BtnInventory, "MODULE_INVENTORY_VIEW" },
                { BtnReports, "MODULE_REPORTS_VIEW" },
                { btnConfiguraciones, "MODULE_CONFIG_VIEW" },
                { btnRepairs, "MODULE_REPAIRS_VIEW" },
            };

            foreach (var item in MapsModules)
            {
                item.Key.Visibility = PermissionManager.Puede(item.Value)
                    ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void btnConfiguraciones_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.ConfigDashboardPage();

            MainFrame.Navigate(confiCards);
        }

        private void btnRepairs_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.RepairManagementPage();

            MainFrame.Navigate(confiCards);
        }

        private void BtnInventory_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.Inventory.InventaryPage();

            MainFrame.Navigate(confiCards);
        }



        private ObservableCollection<NotificationItem> _notifications;

        // ─── Notificaciones ───────────────────────────────────────

        private void InitializeNotifications()
        {
            _notifications = new ObservableCollection<NotificationItem>();

            // Ejemplo: agrega notificaciones reales desde tu BD aquí
            AgregarNotificacion("Reparación lista",
                                "El equipo de Juan Pérez está listo para entrega.",
                                DateTime.Now.AddMinutes(-30));

            AgregarNotificacion("Stock bajo",
                                "Pantallas iPhone 11 — quedan solo 2 unidades.",
                                DateTime.Now.AddHours(-2));

            ActualizarBadge();
        }

        public void AgregarNotificacion(string titulo, string mensaje, DateTime fecha)
        {
            _notifications.Insert(0, new NotificationItem
            {
                Title = titulo,
                Message = mensaje,
                Date = fecha,
                TimeAgo = ObtenerTiempoRelativo(fecha),
                IsRead = false
            });

            ActualizarBadge();
        }

        private void ActualizarBadge()
        {
            int noLeidas = 0;
            foreach (var n in _notifications)
                if (!n.IsRead) noLeidas++;

            if (noLeidas > 0)
            {
                BadgeEllipse.Visibility = Visibility.Visible;
                BadgeCount.Visibility = Visibility.Visible;
                BadgeCount.Text = noLeidas > 9 ? "9+" : noLeidas.ToString();
            }
            else
            {
                BadgeEllipse.Visibility = Visibility.Collapsed;
                BadgeCount.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            NotificationsList.ItemsSource = _notifications;
            NoNotificationsText.Visibility = _notifications.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            NotificationPopup.IsOpen = !NotificationPopup.IsOpen;
        }

        private void BtnMarkAllRead_Click(object sender, RoutedEventArgs e)
        {
            foreach (var n in _notifications)
                n.IsRead = true;

            ActualizarBadge();
            NotificationPopup.IsOpen = false;
        }

        // ─── Logout ───────────────────────────────────────────────

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                SessionManager.Logout();
                PermissionManager.ClearPermission();

                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private static string ObtenerTiempoRelativo(DateTime fecha)
        {
            var diff = DateTime.Now - fecha;
            if (diff.TotalMinutes < 1) return "Ahora mismo";
            if (diff.TotalMinutes < 60) return $"hace {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24) return $"hace {(int)diff.TotalHours} h";
            return $"hace {(int)diff.TotalDays} días";
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.Sales.MenuSalesInvoicePages();

            MainFrame.Navigate(confiCards);
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Windows.Input.Mouse.Synchronize();
        }

        private void BtnDashboard_Checked(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.Dashboard();

            MainFrame.Navigate(confiCards);
        }

        private void BtnToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            // Si el botón está presionado (Menú Contraído)
            if (BtnToggleMenu.IsChecked == true)
            {
                MenuColumn.Width = new GridLength(60); // Ancho compacto para los iconos

                // Ocultamos el texto del LOGO
                TxtLogo.Visibility = Visibility.Collapsed;

                // Ocultamos los paneles de texto de los botones
                CambiarVisibilidadTextos(Visibility.Collapsed);
            }
            else // Si se desmarca (Menú Expandido)
            {
                MenuColumn.Width = new GridLength(250); // Ancho original

                TxtLogo.Visibility = Visibility.Visible;

                // Mostramos los paneles de texto de los botones
                CambiarVisibilidadTextos(Visibility.Visible);
            }
        }

        // Método auxiliar para no repetir código ocultando texto por texto
        private void CambiarVisibilidadTextos(Visibility visibilidad)
        {
            if (PanelTextInicio == null) return; // Evita errores si se ejecuta antes de cargar componentes

            PanelTextInicio.Visibility = visibilidad;
            PanelTextVentas.Visibility = visibilidad;
            PanelTextCreditos.Visibility = visibilidad;
            PanelTextReparaciones.Visibility = visibilidad;
            PanelTextInventario.Visibility = visibilidad;
            PanelTextReportes.Visibility = visibilidad;
            PanelTextConfig.Visibility = visibilidad;
        }

        private void BtnCredits_Click(object sender, RoutedEventArgs e)
        {
            var confiCards = new WpfApp1.Views.Credit.CreditContractsOptionPages();

            MainFrame.Navigate(confiCards);
        }
    }
}
