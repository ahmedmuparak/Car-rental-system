using CarRental.Application.DTOs.Auth;
using CarRental.Application.Interfaces;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

using System.Security.Cryptography;
using System.Text;

namespace CarRental.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IMemoryCache memoryCache;
        private readonly IOTPService otpService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IEmailService emailService,
            IMemoryCache memoryCache,
            IOTPService otpService,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.memoryCache = memoryCache;
            this.otpService = otpService;
            this.httpContextAccessor = httpContextAccessor;
            this.signInManager = signInManager;
            this.otpService = otpService;
            this.roleManager = roleManager;
        }


        public async Task<AuthResponseDTO> LoginAsync(
            LogInDTO model)
        {
            var user =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedAccessException(
                    "Please verify your email before logging in.");
            }

            var passwordValid =
                await userManager.CheckPasswordAsync(
                    user,
                    model.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var token =
                await tokenService.CreateTokenAsync(
                    user.Id,
                    user.Email!,
                    user.FullName);

            return new AuthResponseDTO
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email!
            };
        }


        public async Task<AuthResponseDTO> RegisterAsync(
            RegisterDTO model)
        {
            var existingUser =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (existingUser != null)
            {
                throw new Exception(
                    "Email already exists.");
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false
            };

            var result =
                await userManager.CreateAsync(
                    user,
                    model.Password);

            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description));

                throw new Exception(errors);
            }

            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    AppRoles.Customer);

            if (!roleResult.Succeeded)
            {
                var errors =
                    string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            e => e.Description));

                throw new Exception(errors);
            }


            var otp = otpService.GenerateOtp();


            otpService.SaveOtp(
                user.Email!,
                otp,
                OtpPurpose.EmailVerification);


            await emailService.SendEmailAsync(
                user.Email!,
                "Car Rental - Verify Your Email",
                $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif;">

                    <h2>Welcome to Car Rental</h2>

                    <p>
                        Hello {user.FullName},
                    </p>

                    <p>
                        Thank you for registering with Car Rental.
                    </p>

                    <p>
                        Your email verification code is:
                    </p>

                    <h1 style="
                        letter-spacing: 8px;
                        color: #007bff;
                    ">
                        {otp}
                    </h1>

                    <p>
                        This code will expire in
                        <strong>10 minutes</strong>.
                    </p>

                    <p>
                        If you did not create this account,
                        please ignore this email.
                    </p>

                </body>
                </html>
                """);


            return new AuthResponseDTO
            {
                Token = string.Empty,
                FullName = user.FullName,
                Email = user.Email!
            };
        }


        public async Task ForgotPasswordAsync(
            ForgotPasswordDTO model)
        {
            var user =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (user == null)
            {
                return;
            }


            var otp = otpService.GenerateOtp();


            otpService.SaveOtp(
                user.Email!,
                otp,
                OtpPurpose.PasswordReset);


            var body = $"""
                <!DOCTYPE html>

                <html>

                <body style="font-family: Arial, sans-serif;">

                    <h2>Car Rental - Password Reset</h2>

                    <p>
                        Hello {user.FullName},
                    </p>

                    <p>
                        We received a request to reset your password.
                    </p>

                    <p>
                        Your verification code is:
                    </p>

                    <h1 style="
                        letter-spacing: 8px;
                        color: #007bff;
                    ">
                        {otp}
                    </h1>

                    <p>
                        This code will expire in
                        <strong>10 minutes</strong>.
                    </p>

                    <p>
                        If you did not request a password reset,
                        you can safely ignore this email.
                    </p>

                </body>

                </html>
                """;


            await emailService.SendEmailAsync(
                user.Email!,
                "Car Rental - Password Reset OTP",
                body);
        }


        public async Task<bool> VerifyEmailAsync(
            VerifyEmailDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new Exception(
                    "Email is required.");
            }

            otpService.ValidateOtpFormat(model.OTP);


            var user =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (user == null)
            {
                throw new Exception(
                    "User not found.");
            }


            if (user.EmailConfirmed)
            {
                throw new Exception(
                    "Email is already verified.");
            }


            var isValid =
                otpService.ValidateOtp(
                    user.Email!,
                    model.OTP,
                    OtpPurpose.EmailVerification);


            if (!isValid)
            {
                throw new Exception(
                    "Invalid or expired OTP.");
            }


            user.EmailConfirmed = true;


            var result =
                await userManager.UpdateAsync(user);


            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description));

                throw new Exception(errors);
            }


            otpService.RemoveOtp(
                user.Email!,
                OtpPurpose.EmailVerification);


            return true;
        }


        public async Task<string> VerifyOtpAsync(
            VerifyOtpDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new Exception(
                    "Email is required.");
            }

            otpService.ValidateOtpFormat(model.OTP);


            var user =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid OTP request.");
            }


            var isValid =
                otpService.ValidateOtp(
                    user.Email!,
                    model.OTP,
                    OtpPurpose.PasswordReset);


            if (!isValid)
            {
                throw new Exception(
                    "Invalid or expired OTP.");
            }


            otpService.RemoveOtp(
                user.Email!,
                OtpPurpose.PasswordReset);


            var resetTokenBytes =
                RandomNumberGenerator.GetBytes(32);

            var resetToken =
                Convert.ToBase64String(
                    resetTokenBytes);


            var hashedResetToken =
                otpService.HashValue(resetToken);


            var resetCacheKey =
                otpService.GetResetTokenCacheKey(
                    user.Email!);


            memoryCache.Set(
                resetCacheKey,
                hashedResetToken,
                TimeSpan.FromMinutes(5));


            return resetToken;
        }



        public async Task ResetPasswordAsync(
            ResetPasswordDTO model)
        {
            if (model.NewPassword !=
                model.ConfirmPassword)
            {
                throw new Exception(
                    "Passwords do not match.");
            }


            var user =
                await userManager.FindByEmailAsync(
                    model.Email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid reset request.");
            }


            var resetCacheKey =
                otpService.GetResetTokenCacheKey(
                    user.Email!);


            if (!memoryCache.TryGetValue(
                resetCacheKey,
                out string? storedHashedToken))
            {
                throw new Exception(
                    "Reset session has expired. Please request a new OTP.");
            }


            var hashedInputToken =
                otpService.HashValue(model.ResetToken);


            if (storedHashedToken !=
                hashedInputToken)
            {
                throw new Exception(
                    "Invalid reset token.");
            }


            var identityToken =
                await userManager
                    .GeneratePasswordResetTokenAsync(
                        user);


            var result =
                await userManager.ResetPasswordAsync(
                    user,
                    identityToken,
                    model.NewPassword);


            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        " | ",
                        result.Errors.Select(
                            e =>
                                $"{e.Code}: {e.Description}"));

                throw new Exception(errors);
            }


            memoryCache.Remove(
                resetCacheKey);
        }

        public async Task<UserInfoDTO> UserInfoAsync()
        {
            var principal = httpContextAccessor.HttpContext?.User;

            if (principal == null ||
                principal.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var user = await userManager.GetUserAsync(principal);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var roles = await userManager.GetRolesAsync(user);

            return new UserInfoDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,

                Role = roles.FirstOrDefault()
            };
        }

        public async Task<UserInfoDTO> UpdateUserInfo(
            UpdateUserInfoDTO model)
        {
            var principal =
                httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var user =
                await userManager.GetUserAsync(principal);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "User not found.");
            }

            if (!string.IsNullOrWhiteSpace(model.FullName))
            {
                user.FullName = model.FullName;
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (model.ProfileImage != null &&
                model.ProfileImage.Length > 0)
            {
                var allowedExtensions = new[]
                {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                var extension =
                    Path.GetExtension(
                        model.ProfileImage.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception(
                        "Only JPG, JPEG, PNG and WEBP images are allowed.");
                }

                const long maxFileSize =
                    5 * 1024 * 1024;

                if (model.ProfileImage.Length > maxFileSize)
                {
                    throw new Exception(
                        "Profile image size cannot exceed 5 MB.");
                }

                var uploadsFolder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =
                    $"{Guid.NewGuid()}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using (var stream =
                       new FileStream(
                           filePath,
                           FileMode.Create))
                {
                    await model.ProfileImage
                        .CopyToAsync(stream);
                }

                if (!string.IsNullOrWhiteSpace(
                    user.ProfileImage))
                {
                    var oldImagePath =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            user.ProfileImage.TrimStart('/'));

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                user.ProfileImage =
                    $"/images/profiles/{fileName}";
            }

            var result =
                await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        ", ",
                        result.Errors.Select(
                            e => e.Description));

                throw new Exception(errors);
            }

            return new UserInfoDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = (await userManager.GetRolesAsync(user)).FirstOrDefault(),
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage
            };
        }

        public async Task ChangePassword(ChangePasswordDTO model)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                throw new Exception(
                    "Passwords do not match.");
            }


            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new Exception(
                    "Invalid reset request.");
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, model.CurrentPassword);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid password.");
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

        }


        public async Task AssignRoleAsync(AssignRoleDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new Exception("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Role))
            {
                throw new Exception("Role is required.");
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var validRoles = new[]
            {
        AppRoles.Admin,
        AppRoles.Driver,
        AppRoles.CarOwner,
        AppRoles.Employee,
        AppRoles.Customer
    };

            if (!validRoles.Contains(model.Role))
            {
                throw new Exception("Invalid role.");
            }

            var roleExists = await roleManager.RoleExistsAsync(model.Role);

            if (!roleExists)
            {
                throw new Exception(
                    $"Role '{model.Role}' does not exist.");
            }

            var isAlreadyInRole =
                await userManager.IsInRoleAsync(
                    user,
                    model.Role);

            if (isAlreadyInRole)
            {
                throw new Exception(
                    "User already has this role.");
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                await userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var result = await userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        e => e.Description));

                throw new Exception(errors);
            }
        }

        public async Task<List<UserInfoDTO>> GetAllUsersAsync()
        {
            var users = userManager.Users.ToList();
            var userInfoList = new List<UserInfoDTO>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userInfoList.Add(new UserInfoDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImage = user.ProfileImage,
                    Role = roles.FirstOrDefault()
                });
            }
            return userInfoList;
        }

        public async Task<UserInfoDTO> GetUserByEmailAsync(string email)
        {
            var user = userManager.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var roles = await userManager.GetRolesAsync(user);

            var userInfo = new UserInfoDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Role = roles.FirstOrDefault()
            };

            return userInfo;
        }   

        public async Task LogOut()
        {
            await signInManager.SignOutAsync();
        }
    }
}