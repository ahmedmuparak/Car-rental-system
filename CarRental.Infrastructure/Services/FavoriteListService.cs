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
    public class FavoriteListService: IFavoriteListService
    {
        private readonly CarRentalDbContext context;

        public FavoriteListService(CarRentalDbContext context)
        {
            this.context = context;
        }

        public async Task AddToFavoriteListAsync(string userId, int carId)
        {
            var exists = await context.FavoriteLists
             .AnyAsync(f => f.UserId == userId && f.CarId == carId);

            if (exists) return;

            var car = await context.Cars.AnyAsync(c => c.Id == carId);
            if (!car) throw new ArgumentException("Car Not found.");

            var favorite = new FavoriteList
            {
                UserId = userId,
                CarId = carId,
                CreatedAt = DateTime.UtcNow
            };
            context.FavoriteLists.Add(favorite);
            await context.SaveChangesAsync();
        }

        public async Task RemoveFromFavoriteListAsync(string userId, int carId)
        {
            var favorite = await context.FavoriteLists
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
            if (favorite == null) return;
            var car = await context.Cars.AnyAsync(c => c.Id == carId);
            if (!car) throw new ArgumentException("Car Not found.");

            context.FavoriteLists.Remove(favorite);
            await context.SaveChangesAsync();
        }

        public async Task<List<FavoriteList>> GetFavoriteListByUserIdAsync(string userId)
        {
            return await context.FavoriteLists
                .Include(f => f.Car)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task ClearFavoriteList(string userId)
        {
            var favorites = await context.FavoriteLists.Where(f => f.UserId == userId).ToListAsync();
            context.FavoriteLists.RemoveRange(favorites);
            await context.SaveChangesAsync();
        }
    }
}
