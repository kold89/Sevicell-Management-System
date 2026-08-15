using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ContractCreateDto
    {
        public int ClientId { get; set; }
        public int ProductUnitId { get; set; }
        public int SellerId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DownPayment { get; set; }
        public int InstallmentCount { get; set; }
        public int FrequencyId { get; set; }
        public string FrequencyCode { get; set; } = ""; // SEMANAL / QUINCENAL / MENSUAL
        public DateTime FirstDueDate { get; set; }
        public decimal LateInterestRate { get; set; } // 0.3m, 0.4m, etc.
        public string? Notes { get; set; }
    }

    public class ContractCalculationResult
    {
        public decimal FinancedBalance { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal InstallmentAmount { get; set; }
        public List<InstallmentPreview> Installments { get; set; } = new();
    }

    public class InstallmentPreview
    {
        public int InstallmentNumber { get; set; }
        public decimal ExpectedAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateOnly? PaymentDate { get; set; }

        public string? Status { get; set; }

    }
    public class ContractDetailViewModel : INotifyPropertyChanged
    {
        public string ContractNumber { get; set; }
        public string CreatedAtText { get; set; }
        public string ClientName { get; set; }
        public string ProductName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DownPayment { get; set; }
        public decimal PendingBalance { get; set; }
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        public int PendingInstallmentsCount { get; set; }
        public decimal TotalPaid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
