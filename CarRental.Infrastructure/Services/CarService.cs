using CarRental.Application.DTOs.Car;
using CarRental.Application.DTOs.Pagination;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class CarService:ICarService
    {
        private readonly CarRentalDbContext context;
        private readonly IPaginationService paginationService;
        private readonly ICarImageService carImageService;
        private readonly ICacheService cacheService;

        public CarService(CarRentalDbContext context
            ,IPaginationService paginationService
            ,ICarImageService carImageService
            ,ICacheService cacheService)
        {
            this.context = context;
            this.paginationService = paginationService;
            this.carImageService = carImageService;
            this.cacheService = cacheService;
        }

        public async Task<CarDTO> CreateCarAsync(CreateCarDTO model)
        {

            if (model.Year < 1900 ||
                model.Year > DateTime.UtcNow.Year + 1)
            {
                throw new Exception("Invalid car year.");
            }

            var branchExists =
         await context.Branches
        .AnyAsync(b => b.Id == model.BranchId);


            if (!branchExists)
            {
                throw new Exception(
                    "Branch not found.");
            }

            var ownerExists = await context.Users
                .AnyAsync(u => u.Id == model.OwnerId);

            if (!ownerExists)
            {
                throw new Exception("Owner not found.");
            }

            var imageUrl =
    await carImageService.GetCarImageUrlAsync(
        model.Brand,
        model.Model,
        model.Year,
        "front34");

            var car = new Car
            {
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                Discount = model.Discount,
                PricePerDay = model.PricePerDay,
                Color = model.Color,
                Seats = model.Seats,
                Transmission = model.Transmission,
                FuelType = model.FuelType,
                Status = CarStatus.Available,
                BranchId = model.BranchId,
                OwnerId = model.OwnerId,
                ImageUrl = imageUrl

            };

            await context.Cars.AddAsync(car);

            await context.SaveChangesAsync();

            return MapToDTO(car);
        }

        public async Task<PaginationDTO<CarDTO>> GetAllCars(
            PaginationRequestDTO pagination)
        {
            var pageNumber = pagination.PageNumber < 1
                ? 1
                : pagination.PageNumber;

            var pageSize = pagination.PageSize < 1
                ? 10
                : pagination.PageSize;

            if (pageSize > 100)
                pageSize = 100;

            var cacheKey = $"AllCars_Page_{pageNumber}_Size_{pageSize}";

            var cachedCars =
                await cacheService.GetAsync<PaginationDTO<CarDTO>>(cacheKey);

            if (cachedCars is not null)
            {
                return cachedCars;
            }

            var query = context.Cars.AsNoTracking();

            var totalCount = await query.CountAsync();

            var cars = await query
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarDTO
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    PricePerDay = c.PricePerDay,
                    Color = c.Color,
                    Seats = c.Seats,
                    Transmission = c.Transmission,
                    FuelType = c.FuelType,
                    Status = c.Status,
                    BranchId = c.BranchId,
                    OwnerId = c.OwnerId,
                    Discount = c.Discount ?? 0,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();

            var result = paginationService.Create(
                cars,
                totalCount,
                pageNumber,
                pageSize);

            await cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(5));

            return result;
        }

        public async Task<CarDTO> GetById(int id)
        {
            const string cacheKey = "car";
            var cachedCar = await cacheService.GetAsync<CarDTO>($"{cacheKey}:{id}");
            if (cachedCar != null)
            {
                return cachedCar;
            }
            var car = await context.Cars.FindAsync(id);
            if (car == null)
            {
                throw new Exception("Car not found.");
            }
            await cacheService.SetAsync($"{cacheKey}:{id}", MapToDTO(car), TimeSpan.FromMinutes(10));
            return MapToDTO(car);
        }



        public async Task<CarDTO> UpdateCar(int id, UpdateCarDTO model)
        {
            var car = await context.Cars.FindAsync(id);
            if(car == null)
            {
                throw new Exception("Car not found.");
            }

            var branchExists =
                  await context.Branches
                       .AnyAsync(
                        b => b.Id == model.BranchId);

            if (!branchExists)
            {
                throw new Exception(
                    "Branch not found.");
            }


            if (model.PricePerDay <= 0)
            {
                throw new Exception(
                    "Price per day must be greater than zero.");
            }

            if (model.Seats <= 0)
            {
                throw new Exception(
                    "Seats must be greater than zero.");
            }

            car.Brand = model.Brand;
            car.Model = model.Model;
            car.Year = model.Year;
            car.PricePerDay = model.PricePerDay;
            car.Discount = model.Discount;
            car.Color = model.Color;
            car.Seats = model.Seats;
            car.Transmission = model.Transmission;
            car.FuelType = model.FuelType;
            car.BranchId = model.BranchId;

            await context.SaveChangesAsync();

            return MapToDTO(car);
        }

        public async Task DeleteCar(int id)
        {
            var car = await context.Cars.FindAsync(id);
            if (car == null)
            {
                throw new Exception("Car not found.");
            }
            context.Cars.Remove(car);
            await context.SaveChangesAsync();
        }   

        public async Task<List<CarDTO>> GetAvailableCars()
        {
            const string cacheKey = "cars:available";

            var cachedCars = await cacheService.GetAsync<List<CarDTO>>(cacheKey);

            if (cachedCars is not null)
            {
                return cachedCars;
            }

            var availableCars = await context.Cars
                .Where(c => c.Status == CarStatus.Available)
                .ToListAsync();
            await cacheService.SetAsync(cacheKey, availableCars.Select(MapToDTO).ToList(), TimeSpan.FromMinutes(10));

            return availableCars.Select(MapToDTO).ToList();
        }


        public async Task<List<CarDTO>> GetCarsByBranch(int branchId)
        {
            var branchExists = await context.Branches
                .AnyAsync(b => b.Id == branchId);

            if (!branchExists)
            {
                throw new Exception("Branch not found.");
            }

                var cars = await context.Cars
                .Where(c => c.BranchId == branchId)
                .ToListAsync();
            return 
                cars.Select(MapToDTO).ToList();
        }

        public async Task<CarDTO> UpdateCarStatus(int id, CarStatus status)
        {
            var car = await context.Cars.FindAsync(id);
            if (car == null)
            {
                throw new Exception("Car not found.");
            }

            car.Status = status;
            context.SaveChangesAsync();
            return MapToDTO(car);
        }

        public async Task<List<CarDTO>> SearchCarsAsync(CarSearchDTO model)
        {
            var query = context.Cars
                .Include(c => c.Branch)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(model.Brand))
            {
                query = query.Where(c =>
                    c.Brand.Contains(model.Brand));
            }

            if (!string.IsNullOrWhiteSpace(model.Model))
            {
                query = query.Where(c =>
                    c.Model.Contains(model.Model));
            }

            if (model.BranchId.HasValue)
            {
                query = query.Where(c =>
                    c.BranchId == model.BranchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.City))
            {
                query = query.Where(c =>
                    c.Branch.City.Contains(model.City));
            }

            if (model.MinPrice.HasValue)
            {
                query = query.Where(c =>
                    c.PricePerDay >= model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                query = query.Where(c =>
                    c.PricePerDay <= model.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.FuelType))
            {
                query = query.Where(c =>
                    c.FuelType == model.FuelType);
            }

            if (!string.IsNullOrWhiteSpace(model.Transmission))
            {
                query = query.Where(c =>
                    c.Transmission == model.Transmission);
            }

            if (model.Seats.HasValue)
            {
                query = query.Where(c =>
                    c.Seats == model.Seats.Value);
            }

            if (model.Year.HasValue)
            {
                query = query.Where(c =>
                    c.Year == model.Year.Value);
            }

            if (model.Status.HasValue)
            {
                query = query.Where(c =>
                    c.Status == model.Status.Value);
            }


            if (!string.IsNullOrWhiteSpace(model.SortBy))
            {
                switch (model.SortBy.ToLower())
                {
                    case "price":
                        query = model.Descending
                            ? query.OrderByDescending(c => c.PricePerDay)
                            : query.OrderBy(c => c.PricePerDay);
                        break;

                    case "year":
                        query = model.Descending
                            ? query.OrderByDescending(c => c.Year)
                            : query.OrderBy(c => c.Year);
                        break;

                    case "brand":
                        query = model.Descending
                            ? query.OrderByDescending(c => c.Brand)
                            : query.OrderBy(c => c.Brand);
                        break;

                    default:
                        query = query.OrderBy(c => c.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }


            return query.Select(MapToDTO).ToList();
        }

        private static CarDTO MapToDTO(Car car)
        {
            return new CarDTO
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                PricePerDay = car.PricePerDay,
                Discount = car.Discount ?? 0,
                Color = car.Color,
                Seats = car.Seats,
                Transmission = car.Transmission,
                FuelType = car.FuelType,
                Status = car.Status,
                BranchId = car.BranchId,
                OwnerId = car.OwnerId,
                ImageUrl=car.ImageUrl
            };
        }
    }
}
