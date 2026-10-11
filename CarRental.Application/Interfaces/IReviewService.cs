using CarRental.Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDTO> CreateReview(CreateReviewDTO model, string userId);
        Task<List<ReviewDTO>> GetReviewsForCar(int carId);
        Task<List<ReviewDTO>> GetReviewsForUser(string userId);
        Task<ReviewDTO> GetReviewById(int reviewId);
        Task DeleteReview(int reviewId, string userId);
    }
}
