using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class AuditTable
{
    public int Id { get; set; }

    public DateTime DateCreate { get; set; }

    public int UserId { get; set; }

    public string Accion { get; set; } = null!;

    public string AffectedTable { get; set; } = null!;

    public string? ObjectId { get; set; }

    public string? Details { get; set; }

    public virtual User User { get; set; } = null!;
}
