using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class PurchaseInvoiceDetailViewDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupplierDisplay { get; set; }
        public bool IsFormal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseDetailLineDto> Lines { get; set; } = new();
    }

    /// <summary>
    /// Línea de producto dentro del detalle de una factura.
    /// </summary>
    public class PurchaseDetailLineDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalItem => Quantity * PurchasePrice;
    }

    /// <summary>
    /// Filtros de búsqueda del listado de facturas.
    /// </summary>
    public class InvoiceFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? SupplierId { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    public class PurchaseInvoiceListDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupplierDisplay { get; set; } // nombre proveedor o vendedor casual, ya resuelto
        public bool IsFormal { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

