using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class InventoryMovementDto
    {
        public int ProductId { get; set; }
        public int StockAnterior { get; set; }
        public int Cantidad { get; set; }
        public bool EsIncremento { get; set; }
        public int StockNuevo { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

    }
}
