using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Technician
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
}
