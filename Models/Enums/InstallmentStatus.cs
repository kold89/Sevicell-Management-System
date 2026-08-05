using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Enums
{
    public enum InstallmentStatus
    {
        Pending = 1,
        Paid = 2,
        Overdue = 3,
        Partial = 4,
        PaidLate = 5,
        Waived = 6
    }
}
