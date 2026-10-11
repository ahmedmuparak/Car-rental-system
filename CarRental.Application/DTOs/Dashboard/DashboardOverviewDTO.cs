using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Dashboard
{
    public class DashboardOverviewDTO
    {
        public int TotalUsers { get; set; }
        public int TotalCars { get; set; }
        public int TotalBranches { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
