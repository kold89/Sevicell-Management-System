using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ReciboAdelantoDto
    {
        public DateTime FechaPago { get; set; }
        public int ContractNumber { get; set; }
        public string NombreCliente { get; set; }
        public string NombreProducto { get; set; }
        public decimal MontoTotalAbonado { get; set; }
        public List<DetalleCuotaPagadaDto> CuotasAplicadas { get; set; } = new();
        public decimal SaldoPendienteContrato { get; set; }
        public string RecibidoPor { get; set; }
    }

    public class DetalleCuotaPagadaDto
    {
        public int InstallmentNumber { get; set; }
        public decimal MontoAplicado { get; set; }
        public bool QuedoSaldada { get; set; }
    }
}
