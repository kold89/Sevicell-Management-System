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
using System.Windows.Threading;
using WpfApp1.Security;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        // Timer para actualizar la hora
        private DispatcherTimer _clockTimer;     
        private string name = SessionManager.loggedInUser.Name ?? "";
        public Dashboard()
        {
            InitializeComponent();
            Loaded += Dashboard_Loaded;
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            // Configurar fecha y saludo
            ActualizarSaludoYFecha();

            // Iniciar reloj en tiempo real
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _clockTimer.Tick += (s, args) => ActualizarSaludoYFecha();
            _clockTimer.Start();

            // ============================================
            // DATOS EN DURO (hardcoded) — reemplazar luego
            // ============================================

            // KPI Cards
            LblSalesToday.Text = "L. 4,820";
            LblActiveRepairs.Text = "8";
            LblActiveContracts.Text = "23";
            LblCriticalStock.Text = "4";

            // Totales semanales
            LblSalesWeekTotal.Text = "L. 30,820";
            LblRepairsWeekTotal.Text = "L. 12,400";

            // Estado de reparaciones
            LblStatusInProgress.Text = "5";
            LblStatusWaiting.Text = "2";
            LblStatusReady.Text = "1";

            // Contador listos para entrega
            LblReadyCount.Text = "2 equipos";

            // TODO: Aquí conectarás ScottPlot más adelante
            // Ejemplo:
            // ConfigurarGraficoVentas();
            // ConfigurarGraficoReparaciones();
            // ConfigurarGraficoEstado();
        }

        /// <summary>
        /// Actualiza el saludo según la hora del día y la fecha actual
        /// </summary>
        private void ActualizarSaludoYFecha()
        {
            var ahora = DateTime.Now;
            var hora = ahora.Hour;

            string saludo = hora switch
            {
                >= 5 and < 12 => "Buenos días",
                >= 12 and < 18 => "Buenas tardes",
                _ => "Buenas noches"
            };

            LblGreeting.Text = $"{saludo}, {name} ";
            LblDate.Text = ahora.ToString("dddd, d 'de' MMMM 'de' yyyy",
                new System.Globalization.CultureInfo("es-HN"));
        }

        // ============================================
        // MÉTODOS PARA SCOTTPLOT (descomentar cuando instales el paquete)
        // ============================================
        /*
        private void ConfigurarGraficoVentas()
        {
            // Requiere: dotnet add package ScottPlot.WPF
            // Reemplazar SalesChartPlaceholder con <ScottPlot:WpfPlot x:Name="SalesChart"/>
            
            string[] dias = { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
            double[] ventas = { 3200, 4100, 5800, 4900, 7200, 8500, 6800 };
            
            var plot = SalesChart.Plot;
            var linea = plot.Add.Scatter(dias, ventas);
            linea.LineWidth = 2.5;
            linea.Color = new ScottPlot.Color(33, 150, 243);
            linea.MarkerSize = 6;
            
            // Relleno degradado
            plot.Add.FillY(dias, ventas, 
                System.Linq.Enumerable.Repeat(0.0, 7).ToArray(),
                new ScottPlot.Color(33, 150, 243, 40));
            
            plot.Title("");
            plot.Axes.Left.Label.Text = "";
            plot.Axes.Bottom.Label.Text = "";
            plot.Grid.MajorLineColor = new ScottPlot.Color(232, 236, 240);
            
            SalesChart.Refresh();
        }

        private void ConfigurarGraficoReparaciones()
        {
            string[] dias = { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
            double[] reparaciones = { 2100, 1800, 1500, 1900, 2400, 2200, 2000 };
            
            var plot = RepairsChart.Plot;
            var linea = plot.Add.Scatter(dias, reparaciones);
            linea.LineWidth = 2.5;
            linea.Color = new ScottPlot.Color(243, 156, 18);
            linea.MarkerSize = 6;
            
            plot.Add.FillY(dias, reparaciones,
                System.Linq.Enumerable.Repeat(0.0, 7).ToArray(),
                new ScottPlot.Color(243, 156, 18, 40));
            
            plot.Grid.MajorLineColor = new ScottPlot.Color(232, 236, 240);
            RepairsChart.Refresh();
        }

        private void ConfigurarGraficoEstado()
        {
            double[] valores = { 5, 2, 1 };
            string[] etiquetas = { "En curso", "Esperando", "Listos" };
            
            var plot = StatusChart.Plot;
            var pie = plot.Add.Pie(valores);
            pie.SliceFillColors = new ScottPlot.Color[]
            {
                new(33, 150, 243),
                new(243, 156, 18),
                new(76, 175, 80)
            };
            pie.ExplodeFraction = 0.05;
            pie.DonutFraction = 0.6;
            
            plot.HideAxesAndGrid();
            StatusChart.Refresh();
        }
        */
    }
}

