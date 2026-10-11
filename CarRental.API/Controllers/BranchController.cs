using CarRental.Application.DTOs.Branch;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using CarRental.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService branchService;

        public BranchController(IBranchService branchService)
        {
            this.branchService = branchService;
        }

        [HttpGet("GetAllBranches")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var result = await branchService.GetAllBranchesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranch(CreateBranchDTO model)
        {
            try
            {
                var result = await branchService.CreateBranch(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("GetBranchById/{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            try
            {
                var result = await branchService.GetBranchById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPut("UpdateBranch/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, UpdateBranchDTO model)
        {
            try
            {
                var result = await branchService.UpdateBranch(id, model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpDelete("DeleteBranch/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await branchService.DeleteBranch(id);

                return Ok(new
                {
                    message = "Branch deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
