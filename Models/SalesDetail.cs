using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class SalesDetail
{
    public int Id { get; set; }

    public int? SalesInvoiceId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TaxPercent { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<ProductUnit> ProductUnits { get; set; } = new List<ProductUnit>();

    public virtual SalesInvoice? SalesInvoice { get; set; }
}
