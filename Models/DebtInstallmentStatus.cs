using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class DebtInstallmentStatus
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsExpired { get; set; }

    public bool? IsPaid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<DebtInstallment> DebtInstallments { get; set; } = new List<DebtInstallment>();
}
