using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class RepairDetail
{
    public int Id { get; set; }

    public int? RepairOrderId { get; set; }

    public int? ServiceId { get; set; }

    public int? Quantity { get; set; }

    public decimal? LaborCost { get; set; }

    public decimal? PartCost { get; set; }

    public string? Description { get; set; }

    public virtual RepairOrder? RepairOrder { get; set; }

    public virtual Service? Service { get; set; }
}
