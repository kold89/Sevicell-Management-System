using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Contract
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ProductUnitId { get; set; }

    public int SellerId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public decimal SalePrice { get; set; }

    public decimal DownPayment { get; set; }

    public decimal? FinancedBalance { get; set; }

    public int InstallmentCount { get; set; }

    public decimal? InstallmentAmount { get; set; }

    public int FrequencyId { get; set; }

    public DateOnly FirstDueDate { get; set; }

    public DateOnly? LastDueDate { get; set; }

    public decimal? LateInterestRate { get; set; }

    public int StatusId { get; set; }

    public decimal PendingBalance { get; set; }

    public string? Notes { get; set; }

    public virtual Customer Client { get; set; } = null!;

    public virtual ICollection<ContractPayment> ContractPayments { get; set; } = new List<ContractPayment>();

    public virtual ICollection<DebtInstallment> DebtInstallments { get; set; } = new List<DebtInstallment>();

    public virtual Frequency Frequency { get; set; } = null!;

    public virtual ProductUnit ProductUnit { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;

    public virtual ContractStatus Status { get; set; } = null!;
}
