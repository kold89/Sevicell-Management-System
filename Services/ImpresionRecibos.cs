using System;
using System.Drawing.Printing;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public static class ImpresionRecibos
    {
        public const string Impresora = "POS-58-Series";   // un solo lugar; luego irá a configuración

        public static bool ImpresoraExiste()
        {
            foreach (string p in PrinterSettings.InstalledPrinters)
                if (p.Equals(Impresora, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>Devuelve null si imprimió bien, o el motivo del fallo.</summary>
        public static string? Imprimir(ReciboDto recibo)
        {
            if (!ImpresoraExiste())
                return $"No se encontró la impresora '{Impresora}'.";

            try
            {
                new ReciboPrinterService().ImprimirRecibo(recibo, Impresora);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}