using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.DTOs.Car;

namespace CarRental.Application.DTOs.Branch
{
    public class BranchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public List<CarDTO> Cars { get; set; } = new List<CarDTO>();
    }
}
