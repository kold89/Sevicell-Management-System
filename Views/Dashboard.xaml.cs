using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WpfApp1.Security;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    public partial class Dashboard : Page
    {
        private static readonly CultureInfo Cultura = new("es-HN");
        private static readonly Brush BrushInfo = new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80));
        private static readonly Brush BrushError = new SolidColorBrush(Color.FromRgb(0xC0, 0x39, 0x2B));

        private static readonly Brush BrushAlerta = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
        private static readonly Brush BrushOk = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
        private static readonly Brush BrushValor = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x2E));

        private readonly DashboardService _dashboardService = new();
        private readonly string _name = SessionManager.loggedInUser?.Name ?? "";
        private DispatcherTimer _clockTimer;
        private bool _cargando;

        public Dashboard()
        {
            InitializeComponent();
            Loaded += Dashboard_Loaded;
            Unloaded += Dashboard_Unloaded;
        }

        // ---------- Ciclo de vida ----------
        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            ActualizarSaludoYFecha();

            if (_clockTimer == null)
            {
                _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                _clockTimer.Tick += (s, args) => ActualizarSaludoYFecha();
            }
            _clockTimer.Start();

            await CargarAsync();
        }

        private void Dashboard_Unloaded(object sender, RoutedEventArgs e)
        {
            _clockTimer?.Stop();
        }

        private async void BtnReintentar_Click(object sender, RoutedEventArgs e)
        {
            await CargarAsync();
        }

        // ---------- Carga de datos ----------
        private async Task CargarAsync()
        {
            if (_cargando) return;   // evita cargas solapadas (Loaded + Reintentar)
            _cargando = true;

            MostrarCarga("Actualizando datos...");

            try
            {
                var result = await _dashboardService.ObtenerResumenAsync();

                if (!result.Success || result.Data == null)
                {
                    MostrarError(result.Message);
                    return;
                }

                AplicarDatos(result.Data);
                PnlEstadoCarga.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar el dashboard: " + ex.Message);
            }
            finally
            {
                _cargando = false;
            }
        }

        /// <summary>
        /// Único punto que pinta los datos en pantalla. Cada paso agrega aquí su sección.
        /// </summary>
        private void AplicarDatos(DashboardDto datos)
        {
            // Paso 3: contratos
            LblActiveContracts.Text = datos.ContratosActivos.ToString("N0", Cultura);

            if (datos.CuotasVencidas > 0)
            {
                LblOverdueInstallments.Text = datos.CuotasVencidas == 1
                    ? "1 cuota vencida"
                    : $"{datos.CuotasVencidas:N0} cuotas vencidas";
                LblOverdueInstallments.Foreground = BrushAlerta;
            }
            else
            {
                LblOverdueInstallments.Text = "Sin cuotas vencidas";
                LblOverdueInstallments.Foreground = BrushOk;
            }

            // Paso 4: ventas
            // Paso 5: reparaciones
            // Paso 6: stock
            // Paso 6: stock
            bool hayCritico = datos.StockCritico > 0;

            LblCriticalStock.Text = datos.StockCritico.ToString("N0", Cultura);
            LblCriticalStock.Foreground = hayCritico ? BrushAlerta : BrushValor;

            LblCriticalStockSub.Text = !hayCritico
                ? "Inventario en orden"
                : datos.StockCritico == 1 ? "producto bajo mínimo" : "productos bajo mínimo";
            LblCriticalStockSub.Foreground = hayCritico ? BrushAlerta : BrushOk;
            // Paso 7: listos para entrega
        }

        // ---------- Estado visual de la carga ----------
        private void MostrarCarga(string texto)
        {
            TxtEstadoCarga.Text = texto;
            TxtEstadoCarga.Foreground = BrushInfo;
            BtnReintentar.Visibility = Visibility.Collapsed;
            PnlEstadoCarga.Visibility = Visibility.Visible;
        }

        private void MostrarError(string mensaje)
        {
            TxtEstadoCarga.Text = mensaje;
            TxtEstadoCarga.Foreground = BrushError;
            BtnReintentar.Visibility = Visibility.Visible;
            PnlEstadoCarga.Visibility = Visibility.Visible;
        }

        // ---------- Saludo y fecha ----------
        private void ActualizarSaludoYFecha()
        {
            var ahora = DateTime.Now;

            string saludo = ahora.Hour switch
            {
                >= 5 and < 12 => "Buenos días",
                >= 12 and < 18 => "Buenas tardes",
                _ => "Buenas noches"
            };

            LblGreeting.Text = string.IsNullOrWhiteSpace(_name) ? saludo : $"{saludo}, {_name}";

            string fecha = ahora.ToString("dddd, d 'de' MMMM 'de' yyyy", Cultura);
            LblDate.Text = char.ToUpper(fecha[0], Cultura) + fecha.Substring(1);
        }
    }
}