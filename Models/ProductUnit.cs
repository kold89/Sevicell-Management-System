using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class ProductUnit
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Imei { get; set; } = null!;

    public string? SerialNumber { get; set; }

    public string Status { get; set; } = null!;

    public int? PurchaseDetailId { get; set; }

    public int? SalesDetailId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Imei2 { get; set; } = null!;

    public string? Colour { get; set; }

    public string? Model { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseDetail? PurchaseDetail { get; set; }

    public virtual SalesDetail? SalesDetail { get; set; }
}
