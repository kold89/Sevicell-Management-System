using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? SalePrice { get; set; }

    public int? Stock { get; set; }

    public int? MinimumStock { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public bool? Status { get; set; }

    public string? Code { get; set; }

    public string? ProductDescription { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    public virtual ICollection<SalesDetail> SalesDetails { get; set; } = new List<SalesDetail>();
}
