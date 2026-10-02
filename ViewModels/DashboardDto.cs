using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class DashboardDto
    {
        // Contratos (paso 3)
        public int ContratosActivos { get; set; }
        public int CuotasVencidas { get; set; }
        // Stock (paso 6)
        public int StockCritico { get; set; }
    }
}
