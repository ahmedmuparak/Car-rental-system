using CarRental.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO model);
        Task<AuthResponseDTO> LoginAsync(LogInDTO model);
        Task ForgotPasswordAsync(ForgotPasswordDTO model);

        Task<bool> VerifyEmailAsync(VerifyEmailDTO model);

        Task<string> VerifyOtpAsync(VerifyOtpDTO model);

        Task ResetPasswordAsync(ResetPasswordDTO model);
        Task<UserInfoDTO> UserInfoAsync();
        Task<UserInfoDTO> UpdateUserInfo(UpdateUserInfoDTO model);
        Task ChangePassword(ChangePasswordDTO model);
        Task AssignRoleAsync(AssignRoleDTO model);
        Task<List<UserInfoDTO>> GetAllUsersAsync();
        Task<UserInfoDTO> GetUserByEmailAsync(string email);
        Task LogOut();
    }
}
