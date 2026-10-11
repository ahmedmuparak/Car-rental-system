using CarRental.Application.DTOs.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchDTO>> GetAllBranchesAsync();
        Task<BranchDTO> CreateBranch(CreateBranchDTO model);
        Task<BranchDTO> GetBranchById(int id);
        Task<BranchDTO> UpdateBranch(int id, UpdateBranchDTO model);
        Task DeleteBranch(int id);
    }
}
