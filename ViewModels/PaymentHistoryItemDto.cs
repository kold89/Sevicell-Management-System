using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class PaymentHistoryItemDto
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public string ReceivedBy { get; set; }
        public string? Notes { get; set; }
    }
}
