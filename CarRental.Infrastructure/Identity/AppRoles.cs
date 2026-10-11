using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Identity
{
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string CarOwner = "CarOwner";
        public const string Employee = "Employee";
        public const string Customer = "Customer";
        public const string Driver = "Driver";
    }
}
