using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class SalesInvoice
{
    public int Id { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? CustomerId { get; set; }

    public int? PaymentMethodId { get; set; }

    public bool? Status { get; set; }

    public int? RepairOrderId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual RepairOrder? RepairOrder { get; set; }

    public virtual ICollection<SalesDetail> SalesDetails { get; set; } = new List<SalesDetail>();
}
