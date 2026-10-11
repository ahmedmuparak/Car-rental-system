using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Branch
{
    public class CreateBranchDTO
    {
        [Required(ErrorMessage = "Branch name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Branch Address is required.")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Branch City is required.")]
        public string City { get; set; } = string.Empty;
    }
}
