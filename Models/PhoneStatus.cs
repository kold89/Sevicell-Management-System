using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class PhoneStatus
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }
}
