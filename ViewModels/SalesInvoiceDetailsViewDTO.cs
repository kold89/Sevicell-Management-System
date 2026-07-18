using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class SalesInvoiceDetailsViewDTO
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CustomerDisplay { get; set; }
        public bool IsRegisteredCustomer { get; set; }
        public string PaymentMethodName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SalesDetailLineDto> Lines { get; set; } = new();
    }
    /// <summary>
    /// Fila del listado de ventas.
    /// </summary>
    public class SalesInvoiceListDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CustomerDisplay { get; set; }    // nombre del cliente o "Cliente de mostrador"
        public bool IsRegisteredCustomer { get; set; }  // true = CustomerId tiene valor, false = venta de mostrador
        public decimal? TotalAmount { get; set; }
    }
    public class custumerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    /// <summary>
    /// Tipo de factura para el filtro (1 = Formal/Registrado, 2 = Informal/Mostrador).
    /// </summary>
    public class SalesInvoiceTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// Filtros de búsqueda del listado de ventas.
    /// </summary>
    public class SalesInvoiceFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? TypeFilter { get; set; } // 0/null = todos, 1 = registrado, 2 = mostrador
        public int? PaymentMethodId { get; set; } // null = todos los tipos de venta
        public string? InvoiceNumber { get; set; }
    }
    /// <summary>
    /// Línea de producto dentro de una venta.
    /// </summary>
    public class SalesDetailLineDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalItem => Quantity * UnitPrice;
    }

    /// <summary>
    /// Fila del resumen de ventas agrupado por día (Nivel 1 del reporte diario).
    /// </summary>
    public class DailySalesSummaryDto
    {
        public DateTime Date { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Métricas para las tarjetas superiores (total en efectivo, días con venta, promedio por día).
    /// </summary>
    public class SalesSummaryMetricsDto
    {
        public decimal CashTotal { get; set; }
        public int DistinctDaysCount { get; set; }
        public decimal AveragePerDay { get; set; }
    }

}
