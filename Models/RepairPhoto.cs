using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class RepairPhoto
{
    public int Id { get; set; }

    public int? RepairOrderId { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Description { get; set; }

    public virtual RepairOrder? RepairOrder { get; set; }
}
