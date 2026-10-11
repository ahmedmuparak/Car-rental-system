using CarRental.Application.DTOs.Branch;
using CarRental.Application.DTOs.Car;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class BranchService : IBranchService
    {
        private readonly CarRentalDbContext context;
        private readonly ICacheService cacheService;

        public BranchService(CarRentalDbContext context, ICacheService cacheService)
        {
            this.context = context;
            this.cacheService = cacheService;
        }

        public async Task<List<BranchDTO>> GetAllBranchesAsync()
        {
            const string cacheKey = "all_branches";
            var cachedBranches = await cacheService.GetAsync<List<BranchDTO>>(cacheKey);
            if (cachedBranches != null)
            {
                return cachedBranches;
            }
            var branches = await context.Branches
                .Include(b => b.Cars)
                .ToListAsync();
            await cacheService.SetAsync(cacheKey, branches.Select(MapToDTO).ToList(), TimeSpan.FromMinutes(10));
            return branches.Select(MapToDTO).ToList();
        }


        public async Task<BranchDTO> CreateBranch(CreateBranchDTO model)
        {
            var branch = new Branch
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City
            };
            await context.Branches.AddAsync(branch);
            await context.SaveChangesAsync();
            return MapToDTO(branch);
        }


        public async Task<BranchDTO> GetBranchById(int id)
        {
            const string cacheKey = $"branch";
            
            var cachedBranch = await cacheService.GetAsync<BranchDTO>(cacheKey);
            if (cachedBranch != null)
            {
                return cachedBranch;
            }

            var branch = await context.Branches
                .Include(b =>b.Cars)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }
            await cacheService.SetAsync(cacheKey, MapToDTO(branch), TimeSpan.FromMinutes(10));
            return MapToDTO(branch);
        }

        public async Task<BranchDTO> UpdateBranch(int id, UpdateBranchDTO model)
        {
            var branch = await context.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
            {
                throw new Exception("Branch not found");
            }

            branch.Name = model.Name;
            branch.Address = model.Address;
            branch.City = model.City;

            context.Branches.Update(branch);
            context.SaveChangesAsync();

            return MapToDTO(branch);
        }

        public async Task DeleteBranch(int id)
        {
            var branch = await context.Branches
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                throw new Exception("Branch not found");
            }

            var hasCars = await context.Cars
                .AnyAsync(c => c.BranchId == id);

            if (hasCars)
            {
                throw new Exception("Cannot delete branch because it has cars.");
            }

            context.Branches.Remove(branch);

            await context.SaveChangesAsync();
        }
        private static BranchDTO MapToDTO(Branch branch)
        {
            return new BranchDTO
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                City = branch.City,
                Cars = branch.Cars.Select(c => new CarDTO
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
                    BranchId= c.BranchId,
                    Status = c.Status
                }).ToList()
            };

        }
    }
}