using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class InstallmentPayment
{
    public int Id { get; set; }

    public int InstallmentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? ReceivedBy { get; set; }

    public string? Notes { get; set; }

    public virtual DebtInstallment Installment { get; set; } = null!;
}
