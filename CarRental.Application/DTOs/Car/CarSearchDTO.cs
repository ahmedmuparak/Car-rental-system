using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Car
{
    public class CarSearchDTO
    {
        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int? BranchId { get; set; }

        public string? City { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? FuelType { get; set; }

        public string? Transmission { get; set; }

        public int? Seats { get; set; }

        public int? Year { get; set; }

        public CarStatus? Status { get; set; }


        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;
    }
}
