using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ReciboEngancheDto
    {
        public string NumeroRecibo { get; set; }
        public DateTime FechaPago { get; set; }
        public int ContractNumber { get; set; }
        public string NombreCliente { get; set; }
        public string NombreProducto { get; set; }
        public string Imei { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Enganche { get; set; }
        public decimal SaldoFinanciado { get; set; }
        public decimal TotalAPagar { get; set; }
        public int NumeroCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public string Frecuencia { get; set; }
        public DateTime? PrimerPago { get; set; }
        public string RecibidoPor { get; set; }
    }
}
