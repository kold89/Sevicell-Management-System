using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class PurchaseDetail
{
    public int Id { get; set; }

    public int? PurchaseInvoiceId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? PurchasePrice { get; set; }

    public decimal? TaxPercent { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual Product? Product { get; set; }

    public virtual PurchaseInvoice? PurchaseInvoice { get; set; }
}
