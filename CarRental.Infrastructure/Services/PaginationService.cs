using CarRental.Application.DTOs.Pagination;
using CarRental.Application.Interfaces;

namespace CarRental.Infrastructure.Services
{
    public class PaginationService : IPaginationService
    {
        public PaginationDTO<T> Create<T>(
            List<T> data,
            int totalCount,
            int pageNumber,
            int pageSize)
        {
            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            return new PaginationDTO<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            };
        }
    }
}