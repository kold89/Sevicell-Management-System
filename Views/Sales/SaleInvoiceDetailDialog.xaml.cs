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
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Sales
{
    /// <summary>
    /// Lógica de interacción para SaleInvoiceDetailDialog.xaml
    /// </summary>
    public partial class SaleInvoiceDetailDialog : Window
    {
        public SaleInvoiceDetailDialog(SalesInvoiceDetailsViewDTO dto)
        {
            InitializeComponent();
            CargarDetalle(dto);
        }

        private void CargarDetalle(SalesInvoiceDetailsViewDTO dto)
        {
            TxtDetInvoiceNumber.Text = dto.InvoiceNumber;
            TxtDetFecha.Text = dto.CreatedAt!.Value.ToString("dd/MM/yyyy");
            TxtDetCliente.Text = dto.CustomerDisplay;
            TxtDetTipo.Text = dto.IsRegisteredCustomer ? "Cliente Registrado" : "Venta de Mostrador";
            TxtDetSubtotal.Text = dto.SubTotal.ToString("N2");
            TxtDetDescuento.Text = dto.Discount.ToString("N2");
            TxtDetMetodoPago.Text = dto.PaymentMethodName;
            TxtDetTotal.Text = dto.TotalAmount.ToString("N2");

            DgDetalleLineas.ItemsSource = dto.Lines;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
