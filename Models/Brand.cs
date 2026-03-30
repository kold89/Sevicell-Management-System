using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
