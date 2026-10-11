using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Car
{
    public class UpdateCarDTO
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public decimal PricePerDay { get; set; }
        public decimal Discount { get; set; }
        public string Color { get; set; } = string.Empty;

        public int Seats { get; set; }

        public string Transmission { get; set; } = string.Empty;

        public string FuelType { get; set; } = string.Empty;

        public int BranchId { get; set; }
    }
}
