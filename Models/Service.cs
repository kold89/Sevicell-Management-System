using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Service
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<RepairDetail> RepairDetails { get; set; } = new List<RepairDetail>();

    internal static object UpdateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }
}
