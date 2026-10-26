using CarRental.Application.DTOs.Car;
using CarRental.Application.DTOs.Pagination;
using CarRental.Application.Interfaces;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService carService;

        public CarController(ICarService carService)
        {
            this.carService = carService;
        }


        [Authorize(Roles = AppRoles.Admin)]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles =AppRoles.CarOwner)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCarDTO model)
        {
            try
            {
                var result =
                    await carService
                        .CreateCarAsync(model);

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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestDTO pagination)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                var result = await carService.GetAllCars(pagination);

                stopwatch.Stop();

                Response.Headers["X-Response-Time-ms"] =
                    stopwatch.ElapsedMilliseconds.ToString();
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await carService.GetById(id);
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


        //[Authorize(Roles = AppRoles.Admin)]
        //[Authorize(Roles = AppRoles.SuperAdmin)]
        //[Authorize(Roles = AppRoles.CarOwner)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCarDTO model)
        {
            try
            {
                var result =
                    await carService
                        .UpdateCar(id, model);

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
        [Authorize(Roles = AppRoles.CarOwner)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await carService.DeleteCar(id);
                return Ok(new
                {
                    message = "Car deleted successfully."
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

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCars()
        {
            try
            {
                var result = await carService.GetAvailableCars();
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

        [HttpGet("branch/{branchId:int}")]
        public async Task<IActionResult> GetCarsByBranch(int branchId)
        {
            try
            {
                var result = await carService.GetCarsByBranch(branchId);
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
        [Authorize(Roles = AppRoles.CarOwner)]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateCarStatus(int id, CarStatus status)
        {
            try
            {
                var result = await carService.UpdateCarStatus(id, status);
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchCars([FromQuery] CarSearchDTO model)
        {
            try
            {
                var result = await carService.SearchCarsAsync(model);
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
    }
}
