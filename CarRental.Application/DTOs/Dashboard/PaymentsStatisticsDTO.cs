using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Dashboard
{
    public class PaymentsStatisticsDTO
    {
        public int TotalPayments { get; set; }

        public int PaidPayments { get; set; }

        public int PendingPayments { get; set; }

        public int FailedPayments { get; set; }

        public int RefundedPayments { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
