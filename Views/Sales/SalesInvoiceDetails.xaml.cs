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
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Sales
{
    /// <summary>
    /// Lógica de interacción para SalesInvoiceDetails.xaml
    /// </summary>
    public partial class SalesInvoiceDetails : Page
     {
        private readonly SalesInvoiceServices _salesInvoiceServices = new();
        private readonly PaymentMethodServices _paymentMethodServices = new();
        private int? _paymentMethodFilter = null;

        private class MonthOption
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public SalesInvoiceDetails()
        {
            InitializeComponent();
            InicializarFiltros();
        }

        private async void InicializarFiltros()
        {
            try
            {
                // Meses: 0 = Todos, 1-12 = Enero..Diciembre
                var meses = new List<MonthOption> { new MonthOption { Id = 0, Name = "Todos" } };
                for (int i = 1; i <= 12; i++)
                {
                    meses.Add(new MonthOption
                    {
                        Id = i,
                        Name = System.Globalization.CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(i)
                    });
                }
                CboMes.ItemsSource = meses;
                CboMes.SelectedValue = DateTime.Today.Month; // mes actual por defecto

                // Años: obtenidos de la BD según ventas existentes
                var yearsResult = await _salesInvoiceServices.GetAvailableYearsAsync();
                if (yearsResult.Success && yearsResult.Data.Count > 0)
                {
                    CboAnio.ItemsSource = yearsResult.Data;
                    CboAnio.SelectedIndex = 0; // el año más reciente
                }
                else
                {
                    CboAnio.ItemsSource = new List<int> { DateTime.Today.Year };
                    CboAnio.SelectedIndex = 0;
                }

                // Tipo de venta: métodos de pago existentes (Efectivo, Tarjeta, Crédito, etc.) + "Todos"
                var paymentResult = await _paymentMethodServices.ListAllPaymentMethods(); 
                var tiposVenta = new List<MonthOption> { new MonthOption { Id = 0, Name = "Todos" } };
                if (paymentResult.Success)
                {
                    foreach (var pm in paymentResult.Data)
                    {
                        tiposVenta.Add(new MonthOption { Id = pm.Id, Name = pm.Name }); 
                    }
                }
                CboTipoVenta.ItemsSource = tiposVenta;
                CboTipoVenta.SelectedValue = 0;
                BuscarConFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar filtros: " + ex.Message);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            BuscarConFiltro();
        }

        private (DateTime? desde, DateTime? hasta) ObtenerRangoSeleccionado()
        {
            if (CboAnio.SelectedItem is not int anio)
                return (null, null);

            int mes = CboMes.SelectedValue != null ? Convert.ToInt32(CboMes.SelectedValue) : 0;

            if (mes == 0) // Todos los meses del año seleccionado
            {
                var desde = new DateTime(anio, 1, 1);
                var hasta = new DateTime(anio, 12, 31);
                return (desde, hasta);
            }
            else
            {
                var desde = new DateTime(anio, mes, 1);
                var hasta = desde.AddMonths(1).AddDays(-1);
                return (desde, hasta);
            }
        }

        private async void BuscarConFiltro()
        {
            try
            {
                var (desde, hasta) = ObtenerRangoSeleccionado();

                _paymentMethodFilter = CboTipoVenta.SelectedValue != null ? Convert.ToInt32(CboTipoVenta.SelectedValue) : null;

                var diasResult = await _salesInvoiceServices.GetDailySalesSummaryAsync(desde, hasta, _paymentMethodFilter);
                if (!diasResult.Success)
                {
                    MessageBox.Show(diasResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DgDias.ItemsSource = diasResult.Data;
                DgFacturasDelDia.Visibility = Visibility.Collapsed;
                TxtSinSeleccion.Visibility = Visibility.Visible;

                var metricsResult = await _salesInvoiceServices.GetSalesSummaryMetricsAsync(desde, hasta, _paymentMethodFilter);
                if (metricsResult.Success)
                {
                    var m = metricsResult.Data;
                    TxtTotalEfectivo.Text = "L. " + m.CashTotal.ToString("N2");
                    TxtDiasConVentas.Text = m.DistinctDaysCount.ToString();
                    TxtPromedioPorDia.Text = "L. " + m.AveragePerDay.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }
        }

        // ---------- NIVEL 1 -> NIVEL 2 ----------

        private async void DgDias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgDias.SelectedItem is not DailySalesSummaryDto diaSeleccionado)
                return;

            try
            {
                var result = await _salesInvoiceServices.GetInvoicesByDayAsync(diaSeleccionado.Date, _paymentMethodFilter);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DgFacturasDelDia.ItemsSource = result.Data;

                TxtSinSeleccion.Visibility = Visibility.Collapsed;
                DgFacturasDelDia.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las facturas del día: " + ex.Message);
            }
        }

        // ---------- NIVEL 2 -> NIVEL 3 (ventana modal) ----------

        private async void BtnDetalles_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            if (boton?.DataContext is not SalesInvoiceListDto factura)
                return;

            try
            {
                var result = await _salesInvoiceServices.GetInvoiceDetailAsync(factura.Id);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new SaleInvoiceDetailDialog(result.Data);
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el detalle: " + ex.Message);
            }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}
