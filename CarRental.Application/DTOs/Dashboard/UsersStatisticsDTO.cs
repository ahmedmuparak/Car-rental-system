using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Dashboard
{
    public class UsersStatisticsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalCarOwners { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalAdmins { get; set; }
    }
}
