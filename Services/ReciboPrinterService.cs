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
        private ReciboAdelantoDto _reciboAdelanto;   // nuevo
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

            bool esAbonoParcial = _reciboPago.SaldoCuota.HasValue && _reciboPago.SaldoCuota > 0;

            // Encabezado
            y += DibujarCentrado(g, "SEVICELL", fuenteTitulo, anchoUtil, y);
            y += DibujarCentrado(g, esAbonoParcial ? "COMPROBANTE DE ABONO" : "COMPROBANTE DE PAGO", fuenteNormal, anchoUtil, y);
            y += 10;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Datos del recibo y contrato
            y += DibujarLinea(g, $"Fecha: {_reciboPago.FechaPago:dd/MM/yyyy HH:mm}", fuenteNormal, y);
            y += DibujarLinea(g, $"Cliente: ", fuenteNormal, y);
            y += DibujarLinea(g, $" {_reciboPago.NombreCliente}", fuenteNormal, y);
            y += DibujarLinea(g, $"Artículo: {_reciboPago.NombreProducto}", fuenteChica, y);
            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Detalle del pago
            y += DibujarLinea(g, $"Cuota {_reciboPago.NumeroCuota} de {_reciboPago.TotalCuotas}", fuenteNormal, y);
            y += DibujarLinea(g, $"{(esAbonoParcial ? "Monto abonado" : "Monto pagado")}: L. {_reciboPago.MontoPagado:N2}", fuenteNormal, y);

            // NUEVO: saldo de ESTA cuota, solo si quedó pendiente
            if (esAbonoParcial)
            {
                y += DibujarLinea(g, $"Saldo de esta cuota: L. {_reciboPago.SaldoCuota:N2}", fuenteNormal, y);
            }

            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            // Saldo y siguiente pago (esto ya era el saldo TOTAL del contrato, se mantiene igual)
            y += DibujarLinea(g, $"Saldo pendiente del contrato: ", fuenteTitulo, y);
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
        // NUEVO: impresión del adelanto
        public void ImprimirReciboAdelanto(ReciboAdelantoDto recibo, string nombreImpresora)
        {
            _reciboAdelanto = recibo;

            var doc = new PrintDocument();
            doc.PrinterSettings.PrinterName = nombreImpresora;
            doc.DefaultPageSettings.PaperSize = new PaperSize("Recibo58mm", 220, 1000);
            doc.DefaultPageSettings.Margins = new Margins(5, 5, 5, 5);
            doc.PrintPage += Doc_PrintPageAdelanto;
            doc.Print();
        }
        // NUEVO: impresión del adelanto (varias cuotas en un solo recibo)
        private void Doc_PrintPageAdelanto(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float y = 0;
            float anchoUtil = e.MarginBounds.Width;

            var fuenteTitulo = new Font("Consolas", 11, FontStyle.Bold);
            var fuenteNormal = new Font("Consolas", 9, FontStyle.Regular);
            var fuenteChica = new Font("Consolas", 8, FontStyle.Regular);

            y += DibujarCentrado(g, "SEVICELL", fuenteTitulo, anchoUtil, y);
            y += DibujarCentrado(g, "COMPROBANTE DE ADELANTO", fuenteNormal, anchoUtil, y);
            y += 10;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            y += DibujarLinea(g, $"Fecha: {_reciboAdelanto.FechaPago:dd/MM/yyyy HH:mm}", fuenteNormal, y);
            y += DibujarLinea(g, $"Contrato No: {_reciboAdelanto.ContractNumber}", fuenteNormal, y);
            y += DibujarLinea(g, $"Cliente: ", fuenteNormal, y);
            y += DibujarLinea(g, $" {_reciboAdelanto.NombreCliente}", fuenteNormal, y);
            y += DibujarLinea(g, $"Artículo: {_reciboAdelanto.NombreProducto}", fuenteChica, y);
            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            y += DibujarLinea(g, $"Monto total abonado: L. {_reciboAdelanto.MontoTotalAbonado:N2}", fuenteNormal, y);
            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            y += DibujarLinea(g, "Detalle de cuotas aplicadas:", fuenteChica, y);
            y += 3;

            foreach (var cuota in _reciboAdelanto.CuotasAplicadas)
            {
                string estado = cuota.QuedoSaldada ? "SALDADA" : "PARCIAL";
                y += DibujarLinea(g, $" Cuota {cuota.InstallmentNumber}: L. {cuota.MontoAplicado:N2} [{estado}]", fuenteChica, y);
            }

            y += 5;
            g.DrawLine(Pens.Black, 0, y, anchoUtil, y);
            y += 5;

            y += DibujarLinea(g, $"Saldo pendiente del contrato: ", fuenteTitulo, y);
            y += DibujarLinea(g, $" L. {_reciboAdelanto.SaldoPendienteContrato:N2}", fuenteTitulo, y);
            y += 5;

            if (_reciboAdelanto.SaldoPendienteContrato <= 0)
            {
                y += DibujarCentrado(g, "¡CONTRATO LIQUIDADO!", fuenteTitulo, anchoUtil, y);
                y += 5;
            }

            y += 10;
            y += DibujarLinea(g, $"Recibido por: {_reciboAdelanto.RecibidoPor}", fuenteChica, y);
            y += 10;
            y += DibujarCentrado(g, "¡Gracias por su pago!", fuenteChica, anchoUtil, y);

            e.HasMorePages = false;
        }

    }
}
