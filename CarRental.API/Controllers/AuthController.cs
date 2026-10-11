using CarRental.Application.DTOs.Auth;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDTO model)
        {
            try
            {
                var result =
                    await authService.RegisterAsync(model);

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



        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            VerifyEmailDTO model)
        {
            try
            {
                await authService.VerifyEmailAsync(model);

                return Ok(new
                {
                    message =
                        "Email verified successfully."
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



        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LogInDTO model)
        {
            try
            {
                var result =
                    await authService.LoginAsync(model);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordDTO model)
        {
            try
            {
                await authService.ForgotPasswordAsync(model);

                return Ok(new
                {
                    message =
                        "If the email exists, an OTP has been sent."
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


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
            VerifyOtpDTO model)
        {
            try
            {
                var resetToken =
                    await authService.VerifyOtpAsync(model);

                return Ok(new
                {
                    message =
                        "OTP verified successfully.",

                    resetToken = resetToken
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


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordDTO model)
        {
            try
            {
                await authService.ResetPasswordAsync(model);

                return Ok(new
                {
                    message =
                        "Password has been reset successfully."
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

        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> UserInfo()
        {
            try
            {
                var userInfo =
                    await authService.UserInfoAsync();

                return Ok(userInfo);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        [Authorize]
        [HttpPut("user-info")]
        public async Task<IActionResult> UpdateUserInfo(
            [FromForm] UpdateUserInfoDTO model)
        {
            try
            {
                var result =
                    await authService.UpdateUserInfo(model);

                return Ok(new
                {
                    message =
                        "User information updated successfully.",

                    data = new
                    {
                        result.FullName,
                        result.PhoneNumber
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDTO model)
        {
            try
            {

                await authService.ChangePassword(model);
                return Ok(new
                {
                    message =
                        "Password changed successfully."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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


        [Authorize(Roles =AppRoles.SuperAdmin)]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleDTO model)
        {
            try
            {
                await authService.AssignRoleAsync(model);
                return Ok(new
                {
                    message = $"Role '{model.Role}' assigned to user '{model.Email}' successfully."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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


        [Authorize(Roles = AppRoles.SuperAdmin)]
        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("user-by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            try
            {
                var user = await authService.GetUserByEmailAsync(email);
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await authService.LogOut();
                return Ok(new
                {
                    message = "Logged out successfully."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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