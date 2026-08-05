using System;
using System.Collections.Generic;
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
        public decimal LateInterestRate { get; set; } // 0.03m, 0.04m, etc.
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
    }
}
