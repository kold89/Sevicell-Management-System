using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class DebtInstallment
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public int InstallmentNumber { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal ExpectedAmount { get; set; }

    public decimal? LateInterest { get; set; }

    public decimal? PaidAmount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public int StatusId { get; set; }

    public int? ReceivedBy { get; set; }

    public string? Notes { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual ICollection<ContractPayment> ContractPayments { get; set; } = new List<ContractPayment>();

    public virtual ICollection<InstallmentPayment> InstallmentPayments { get; set; } = new List<InstallmentPayment>();

    public virtual User? ReceivedByNavigation { get; set; }

    public virtual DebtInstallmentStatus Status { get; set; } = null!;
}
