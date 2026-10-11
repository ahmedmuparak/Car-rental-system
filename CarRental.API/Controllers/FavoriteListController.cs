using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CarRental.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteListController : ControllerBase
    {
        private readonly IFavoriteListService favoriteListService;

        public FavoriteListController(IFavoriteListService favoriteListService)
        {
            this.favoriteListService = favoriteListService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavoriteList(int carId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                await favoriteListService.AddToFavoriteListAsync(userId, carId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{carId}")]
        public async Task<IActionResult> RemoveFromFavoriteList(int carId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                await favoriteListService.RemoveFromFavoriteListAsync(userId, carId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteListByUserId()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                var favoriteList = await favoriteListService.GetFavoriteListByUserIdAsync(userId);
                return Ok(favoriteList);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearFavoriteList()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated"
                    });
                }
                await favoriteListService.ClearFavoriteList(userId);
                return Ok();
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
