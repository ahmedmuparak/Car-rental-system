using CarRental.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IPaginationService
    {
        PaginationDTO<T> Create<T>(
            List<T> data,
            int totalCount,
            int pageNumber,
            int pageSize);
    }
}
