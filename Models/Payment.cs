using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? SalesInvoiceId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual SalesInvoice? SalesInvoice { get; set; }

    public virtual User? User { get; set; }
}
