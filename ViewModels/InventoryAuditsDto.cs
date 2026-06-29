using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class InventoryAuditsDto
    {
        public int Folio { get; set; }
        public string ProductoNombre { get; set; }
        public int StockAnterior { get; set; }
        public string CantidadConSigno { get; set; }
        public bool EsIncremento { get; set; }
        public int StockNuevo { get; set; }
        public string Motivo { get; set; }
        public DateTime? Fecha { get; set; }
        public string UsuarioResponsable { get; set; }
    }
}
