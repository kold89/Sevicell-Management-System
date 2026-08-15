using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class ReciboPrinterService
    {
        private ReciboPagoCuotaDto _reciboPago;
        private float DibujarLinea(Graphics g, string texto, Font fuente, float y)
        {
            g.DrawString(texto, fuente, Brushes.Black, 0, y);
            return fuente.GetHeight(g) + 2;
        }

        private float DibujarCentrado(Graphics g, string texto, Font fuente, float anchoUtil, float y)
        {
            var tam = g.MeasureString(texto, fuente);
            float x = (anchoUtil - tam.Width) / 2;
            if (x < 0) x = 0;
            g.DrawString(texto, fuente, Brushes.Black, x, y);
            return fuente.GetHeight(g) + 2;
        }
        public void ImprimirReciboPago(ReciboPagoCuotaDto recibo, string nombreImpresora)
        {
            _reciboPago = recibo;

            var doc = new PrintDocument();
            doc.PrinterSettings.PrinterName = nombreImpresora;
            doc.DefaultPageSettings.PaperSize = new PaperSize("Recibo58mm", 220, 1000);
            doc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);
            doc.PrintPage += Doc_PrintPagePago;
            doc.Print();
        }

        private void Doc_PrintPagePago(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float y = 0;
            float anchoUtil = e.MarginBounds.Width;

            var fuenteTitulo = new Font("Consolas", 11, FontStyle.Bold);
            var fuenteNormal = new Font("Consolas", 9, FontStyle.Regular);
            var fuenteChica = new Font("Consolas", 8, FontStyle.Regular);

            // Encabezado
            y += DibujarCentrado(g, "SEVICELL", fuenteTitulo, anchoUtil, y);
            y += DibujarCentrado(g, "COMPROBANTE DE PAGO", fuenteNormal, anchoUtil, y);
            y += 10;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Datos del recibo y contrato
            //y += DibujarLinea(g, $"Recibo No: {_reciboPago.NumeroRecibo}", fuenteNormal, y);
            y += DibujarLinea(g, $"Fecha: {_reciboPago.FechaPago:dd/MM/yyyy HH:mm}", fuenteNormal, y);
            //y += DibujarLinea(g, $"Contrato No: {_reciboPago.ContractNumber}", fuenteNormal, y);
            y += DibujarLinea(g, $"Cliente: ", fuenteNormal, y);
            y += DibujarLinea(g, $" {_reciboPago.NombreCliente}", fuenteNormal, y);
            y += DibujarLinea(g, $"Artículo: {_reciboPago.NombreProducto}", fuenteChica, y);
            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Detalle del pago
            y += DibujarLinea(g, $"Cuota {_reciboPago.NumeroCuota} de {_reciboPago.TotalCuotas}", fuenteNormal, y);
            y += DibujarLinea(g, $"Monto pagado: L. {_reciboPago.MontoPagado:N2}", fuenteNormal, y);
            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Saldo y siguiente pago
            y += DibujarLinea(g, $"Saldo pendiente: ", fuenteTitulo, y);
            y += DibujarLinea(g, $" L. {_reciboPago.SaldoPendiente:N2}", fuenteTitulo, y);
            y += 5;

            if (_reciboPago.ProximaFechaPago.HasValue)
            {
                y += DibujarLinea(g, $"Próximo pago: {_reciboPago.ProximaFechaPago:dd/MM/yyyy}", fuenteChica, y);
            }
            else
            {
                y += DibujarCentrado(g, "¡CONTRATO LIQUIDADO!", fuenteTitulo, anchoUtil, y);
            }

            y += 10;
            y += DibujarLinea(g, $"Recibido por: {_reciboPago.RecibidoPor}", fuenteChica, y);
            y += 10;
            y += DibujarCentrado(g, "¡Gracias por su pago!", fuenteChica, anchoUtil, y);

            e.HasMorePages = false;
        }
    }
}
