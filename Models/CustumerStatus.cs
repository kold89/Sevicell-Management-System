using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class CustumerStatus
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
