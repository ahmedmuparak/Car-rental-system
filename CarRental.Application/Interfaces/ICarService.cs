using CarRental.Application.DTOs.Car;
using CarRental.Application.DTOs.Pagination;
using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface ICarService
    {
        Task<CarDTO> CreateCarAsync(CreateCarDTO model);
        Task<PaginationDTO<CarDTO>> GetAllCars(
            PaginationRequestDTO pagination);
        Task<CarDTO> GetById(int id);
        Task<CarDTO> UpdateCar(int id, UpdateCarDTO model);
        Task DeleteCar(int id);
        Task<List<CarDTO>> GetAvailableCars();
        Task<List<CarDTO>> GetCarsByBranch(int branchId);
        Task<CarDTO> UpdateCarStatus(int id, CarStatus status);
        Task<List<CarDTO>> SearchCarsAsync(CarSearchDTO model);
    }
}
