using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class ContractPayment
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public int? InstallmentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public int? ReceivedBy { get; set; }

    public string? Notes { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual DebtInstallment? Installment { get; set; }
}
