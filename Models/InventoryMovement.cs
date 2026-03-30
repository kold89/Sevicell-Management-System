using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class InventoryMovement
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? MovementType { get; set; }

    public int? Quantity { get; set; }

    public DateTime? MovementDate { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
