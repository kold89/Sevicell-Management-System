using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class PurchaseInvoice
{
    public int Id { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Tax { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? SupplierId { get; set; }

    public bool? InoviceNumberExist { get; set; }

    public string? SupplierNameCasual { get; set; }

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual Supplier? Supplier { get; set; }
}
