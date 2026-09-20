using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class CardStatusInstallment
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string DarkColor { get; set; }
        public int Count { get; set; }
    }

    public class CobroItemDto
    {
        public int InstallmentId { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string ClientName { get; set; }
        public string ProductName { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal ExpectedAmount { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; } // negativo o 0 si no está vencida
        public string UrgencyGroup { get; set; } // OVERDUE, TODAY, WEEK, MONTH
        public decimal PaidAmount { get; set; }
        public bool IsPartial { get; set; }
        public decimal SaldoPendiente => ExpectedAmount - PaidAmount;
    }

}
