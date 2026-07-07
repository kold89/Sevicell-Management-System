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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para PurchaseInvoiceListPage.xaml
    /// </summary>
    public partial class PurchaseInvoiceListPage : Page
    {
        private readonly PurchaseInvoiceServices _purchaseInvoiceServices = new();
        private readonly SupplierServices _supplierServices = new();
        private DateTime? _fechaDesde = null;
        private DateTime? _fechaHasta = null;

        public PurchaseInvoiceListPage()
        {
            InitializeComponent();
            LoadSuppliersFilter();
            //BuscarFacturas();
        }

        private async void LoadSuppliersFilter()
        {
            try
            {
                var data = await _supplierServices.listSupplierForGrid();
                if (!data.Success) return;

                var suppliers = new List<SupplierDto> { new SupplierDto { id = 0, name = "-- Todos --" } };
                suppliers.AddRange(data.Data);

                CboFiltroProveedor.ItemsSource = suppliers;
                CboFiltroProveedor.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }

        private async void DgFacturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgFacturas.SelectedItem is not PurchaseInvoiceListDto seleccionada)
                return;

            try
            {
                var result = await _purchaseInvoiceServices.GetInvoiceDetailAsync(seleccionada.Id);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dto = result.Data;

                TxtDetInvoiceNumber.Text = dto.InvoiceNumber;
                TxtDetFecha.Text = dto.CreatedAt.ToString("dd/MM/yyyy");
                TxtDetProveedor.Text = dto.SupplierDisplay;
                TxtDetTipo.Text = dto.IsFormal ? "Compra Formal" : "Compra Informal";
                TxtDetSubtotal.Text = dto.SubTotal.ToString("N2");
                TxtDetImpuesto.Text = dto.Tax.ToString("N2");
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

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filter = new InvoiceFilterDto
                {
                    DateFrom = _fechaDesde,   // 👈 antes: DpDesde.SelectedDate
                    DateTo = _fechaHasta,     // 👈 antes: DpHasta.SelectedDate
                    SupplierId = CboFiltroProveedor.SelectedValue != null && Convert.ToInt32(CboFiltroProveedor.SelectedValue) > 0
                        ? Convert.ToInt32(CboFiltroProveedor.SelectedValue) : null,
                    InvoiceNumber = string.IsNullOrWhiteSpace(TxtFiltroNumero.Text) ? null : TxtFiltroNumero.Text.Trim()
                };

                var result = await _purchaseInvoiceServices.GetInvoicesAsync(filter);
                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DgFacturas.ItemsSource = result.Data;
                PanelDetalle.Visibility = Visibility.Collapsed;
                TxtSinSeleccion.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }

        }
    }
}
