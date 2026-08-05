using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class ContractStatus
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string? ColorBadge { get; set; }

    public int FlowOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
