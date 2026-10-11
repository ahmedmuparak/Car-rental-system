using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface ICarImageService
    {
        Task<string?> GetCarImageUrlAsync(
            string make,
            string model,
            int year,
            string? view = null);
    }
}
