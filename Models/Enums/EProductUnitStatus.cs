using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Enums
{
    public enum EProductUnitStatus
    {
        Disponible,
        Vendido 
    }

    public static class ProductUnitStatusExtensions
    {
        public static string ToDbValue(this EProductUnitStatus status) => status switch
        {
            EProductUnitStatus.Disponible => "Disponible",
            EProductUnitStatus.Vendido => "Vendido",
            _ => throw new ArgumentException("Estado no reconocido")
        };
    }
}
