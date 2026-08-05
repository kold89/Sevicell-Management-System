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
    /// Lógica de interacción para SalesInovoiceListPage.xaml
    /// </summary>
    public partial class SalesInovoiceListPage : Page
    {
        private DateTime? _fechaDesde = null;
        private DateTime? _fechaHasta = null;
        private readonly SalesInvoiceServices serviceSalesInvoice = new SalesInvoiceServices();

        public SalesInovoiceListPage()
        {
            InitializeComponent();
            LoadTypeInovice();
            BuscarFacturas();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }

        private void BorderFechaDesde_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupCalendarioDesde.IsOpen = true;
        }

        private void CalFechaDesde_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalFechaDesde.SelectedDate.HasValue)
            {
                _fechaDesde = CalFechaDesde.SelectedDate.Value;
                TxtFechaDesdeSeleccionada.Text = _fechaDesde.Value.ToString("dd/MM/yyyy");
                TxtFechaDesdeSeleccionada.Foreground = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B));
                PopupCalendarioDesde.IsOpen = false;
            }
        }

        private void BorderFechaHasta_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupCalendarioHasta.IsOpen = true;
        }

        private void CalFechaHasta_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalFechaHasta.SelectedDate.HasValue)
            {
                _fechaHasta = CalFechaHasta.SelectedDate.Value;
                TxtFechaHastaSeleccionada.Text = _fechaHasta.Value.ToString("dd/MM/yyyy");
                TxtFechaHastaSeleccionada.Foreground = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B));
                PopupCalendarioHasta.IsOpen = false;
            }
        }
        private void LoadTypeInovice()
        {
            try
            {
                var data = serviceSalesInvoice.GetTypeSalesInvoice();
                
                if (!data.Success) return;

                CboFilterTypeInvoice.ItemsSource = data.Data;
                CboFilterTypeInvoice.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipo de factura: " + ex.Message);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            BuscarFacturas();
        }
        private async void BuscarFacturas()
        {
            try
            {
                var filter = new SalesInvoiceFilterDto
                {
                    DateFrom = _fechaDesde,
                    DateTo = _fechaHasta,
                    TypeFilter = CboFilterTypeInvoice.SelectedValue != null && Convert.ToInt32(CboFilterTypeInvoice.SelectedValue) > 0
                        ? Convert.ToInt32(CboFilterTypeInvoice.SelectedValue) : null,
                    InvoiceNumber = string.IsNullOrWhiteSpace(TxtFiltroNumero.Text) ? null : TxtFiltroNumero.Text.Trim()
                };

                var result = await serviceSalesInvoice.GetInvoicesAsync(filter);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DgFacturasVentas.ItemsSource = result.Data;
                PanelDetalle.Visibility = Visibility.Collapsed;
                TxtSinSeleccion.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }

        private async void DgFacturasVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgFacturasVentas.SelectedItem is not SalesInvoiceListDto seleccionada)
                return;

            try
            {
                var result = await serviceSalesInvoice.GetInvoiceDetailAsync(seleccionada.Id);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dto = result.Data;

                TxtDetInvoiceNumber.Text = dto.InvoiceNumber;
                TxtDetFecha.Text = Convert.ToString(dto.CreatedAt);
                TxtDetCliente.Text = dto.CustomerDisplay;
                TxtDetTipo.Text = dto.IsRegisteredCustomer ? "Cliente Registrado" : "Venta de Mostrador";
                TxtDetSubtotal.Text = dto.SubTotal.ToString("N2");
                TxtDetDescuento.Text = dto.Discount.ToString("N2");
                TxtDetMetodoPago.Text = dto.PaymentMethodName;
                TxtDetTotal.Text = dto.TotalAmount.ToString("N2");

                DgDetalleLineas.ItemsSource = dto.Lines;

                TxtSinSeleccion.Visibility = Visibility.Collapsed;
                PanelDetalle.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle: " + ex.Message);
            }
        }

        private void CboFilterTypeInvoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
