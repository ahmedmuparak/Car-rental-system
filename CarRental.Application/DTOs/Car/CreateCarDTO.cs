using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Car
{
    public class CreateCarDTO
    {
        [Required(ErrorMessage = "Car Brand is required.")]

        public string Brand { get; set; } = string.Empty;
        [Required(ErrorMessage = "Car model is required.")]
        public string Model { get; set; } = string.Empty;

        public decimal Discount { get; set; }
        public int Year { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price per day must be greater than zero.")]
        public decimal PricePerDay { get; set; }

        public string Color { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Seats must be greater than zero.")]

        public int Seats { get; set; }

        public string Transmission { get; set; } = string.Empty;

        public string FuelType { get; set; } = string.Empty;

        public int BranchId { get; set; }

        public string? OwnerId { get; set; }
    }
}
