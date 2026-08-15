using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class TemplateContractDto
    {
        // Datos del Vendedor
        public string NombreVendedor { get; set; } = "HUMBERTO";
        public string DniVendedor { get; set; } = "xxxx-xxxx-xxxxx";
        public string empresa { get; set; }

        // Datos del Comprador
        public string NombreComprador { get; set; } = string.Empty;
        public string DniComprador { get; set; } = string.Empty;
        public string DomicilioComprador { get; set; } = string.Empty;

        // Detalles del Dispositivo Mueble
        public string Articulo { get; set; } = "Teléfono Celular";
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Imei { get; set; } = string.Empty;
        public string Imei2 { get; set; } = string.Empty;

        // Valores y Parámetros Financieros (Tipos Numéricos Correctos)
        public decimal PrecioTotal { get; set; }
        public decimal Prima { get; set; }
        public decimal SaldoFinanciado => PrecioTotal - Prima; // Se calcula sola
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public string FrecuenciaPago { get; set; } = "Mensual";

        // Fechas e Información Geográfica
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public string Municipio { get; set; } = "Teupasenti";
        public string Departamento { get; set; } = "El Paraíso";
        public DateTime FechaFirma { get; set; } = DateTime.Now;
    }
}
