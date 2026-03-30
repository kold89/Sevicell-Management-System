using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Device
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? BrandId { get; set; }

    public string? Model { get; set; }

    public string? Imei { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
}
