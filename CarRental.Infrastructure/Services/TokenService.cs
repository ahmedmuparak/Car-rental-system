using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public TokenService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(
            string userId,
            string email,
            string fullName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName ?? ""),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    "FullName",
                    fullName)
            };

            var roles =
                await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration["JwtSettings:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:
                    configuration["JwtSettings:Issuer"],

                audience:
                    configuration["JwtSettings:Audience"],

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(
                        double.Parse(
                            configuration[
                                "JwtSettings:DurationInMinutes"]!)),

                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}