using CarRental.Application.DTOs.Review;
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
    public class ReviewService : IReviewService
    {
        private readonly CarRentalDbContext context;

        public ReviewService(CarRentalDbContext context)
        {
            this.context = context;
        }

        public async Task<ReviewDTO> CreateReview(CreateReviewDTO model, string userId)
        {
            var User = await context.Users.FindAsync(userId);

            if (User == null)
            {
                throw new Exception("User not found");
            }

            var car = await context.Cars.FindAsync(model.CarId);
            if (car == null)
            {
                throw new Exception("Car not found");
            }

            var booking = await context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.Id == model.BookingId &&
                    b.CustomerId == userId);

            if (booking == null)
            {
                throw new Exception(
                    "Booking not found or does not belong to this user");
            }

            if (booking.CarId != model.CarId)
            {
                throw new Exception(
                    "This booking does not belong to this car");
            }

            if (booking.Status != BookingStatus.Completed)
            {
                throw new Exception(
                    "You can only review a completed booking");
            }

            if (model.Rating < 1 || model.Rating > 5)
            {
                throw new Exception(
                    "Rating must be between 1 and 5");
            }

            var existingReview = await context.Reviews
    .AnyAsync(r =>
        r.UserId == userId &&
        r.BookingId == model.BookingId);

            if (existingReview)
            {
                throw new Exception(
                    "You have already reviewed this booking");
            }


            var review = new Review
            {
                UserId = userId,
                CarId = model.CarId,
                BookingId = model.BookingId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review);

            await context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                UserId = userId,
                UserName = User.FullName,
                CarId = review.CarId,
                BookingId = review.BookingId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }


        public async Task<List<ReviewDTO>> GetReviewsForCar(int carId)
        {
            var car = await context.Cars.FindAsync(carId);
            if (car == null)
            {
                throw new Exception("Car not found");
            }

            var reviews = await context.Reviews
                .Where(r => r.CarId == carId)
                .Include(r => r.Booking)
                .Include(r => r.Car)
                .ToListAsync();

            if (reviews.Count == 0)
            {
                throw new Exception("No reviews found for this car");
            }
            var reviewDTOs = reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = context.Users
                    .Where(u => u.Id == r.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? string.Empty,
                CarId = r.CarId,
                BookingId = r.BookingId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
            return reviewDTOs;
        }

        public Task<List<ReviewDTO>> GetReviewsForUser(string userId)
        {
            var reviews = context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.Booking)
                .Include(r => r.Car)
                .ToList();
            if (reviews.Count == 0)
            {
                throw new Exception("No reviews found for this user");
            }
            var reviewDTOs = reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = context.Users
                    .Where(u => u.Id == r.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? string.Empty,
                CarId = r.CarId,
                BookingId = r.BookingId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
            return Task.FromResult(reviewDTOs);
        }

        public Task<ReviewDTO> GetReviewById(int reviewId)
        {
            var review = context.Reviews
                .Where(r => r.Id == reviewId)
                .Include(r => r.Booking)
                .Include(r => r.Car)
                .FirstOrDefault();
            if (review == null)
            {
                throw new Exception("Review not found");
            }
            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = context.Users
                    .Where(u => u.Id == review.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? string.Empty,
                CarId = review.CarId,
                BookingId = review.BookingId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
            return Task.FromResult(reviewDTO);
        }

        public async Task DeleteReview(int reviewId, string userId)
        {
            var review = await context.Reviews
                .Where(r => r.Id == reviewId && r.UserId == userId)
                .FirstOrDefaultAsync();
            if (review == null)
            {
                throw new Exception("Review not found or does not belong to this user");
            }
            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
        }
    }
}
