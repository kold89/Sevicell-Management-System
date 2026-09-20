using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ReciboPagoCuotaDto
    {
        public string NumeroRecibo { get; set; }
        public DateTime FechaPago { get; set; }

        public int ContractNumber { get; set; }
        public string NombreCliente { get; set; }
        public string NombreProducto { get; set; }   // el artículo financiado, ej. "Teléfono Samsung A15"

        public int NumeroCuota { get; set; }          // ej. "Cuota 3 de 12"
        public int TotalCuotas { get; set; }

        public decimal MontoPagado { get; set; }
        public decimal? SaldoCuota { get; set; }
        public decimal SaldoPendiente { get; set; }   // lo que queda del contrato después de este pago
        public DateTime? ProximaFechaPago { get; set; } // null si esta era la última cuota

        public string RecibidoPor { get; set; }        // usuario/cajero que registró el pago
    }
}
