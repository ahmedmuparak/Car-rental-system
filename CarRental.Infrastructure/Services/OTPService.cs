using CarRental.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.DTOs.Auth;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Identity;

namespace CarRental.Infrastructure.Services
{
    public class OTPService: IOTPService
    {
        private readonly IMemoryCache memoryCache;

        public OTPService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(
                    100000,
                    1000000)
                .ToString();
        }


        public void SaveOtp(
            string email,
            string otp,
            OtpPurpose purpose)
        {
            var cacheKey =
                GetOtpCacheKey(
                    email,
                    purpose);


            var hashedOtp =
                HashValue(otp);


            memoryCache.Set(
                cacheKey,
                hashedOtp,
                TimeSpan.FromMinutes(10));
        }


        public bool ValidateOtp(
            string email,
            string otp,
            OtpPurpose purpose)
        {
            var cacheKey =
                GetOtpCacheKey(
                    email,
                    purpose);


            if (!memoryCache.TryGetValue(
                cacheKey,
                out string? storedHashedOtp))
            {
                return false;
            }


            var hashedInputOtp =
                HashValue(otp);


            return storedHashedOtp ==
                   hashedInputOtp;
        }


        public void RemoveOtp(
            string email,
            OtpPurpose purpose)
        {
            var cacheKey =
                GetOtpCacheKey(
                    email,
                    purpose);

            memoryCache.Remove(cacheKey);
        }


        public string GetOtpCacheKey(
            string email,
            OtpPurpose purpose)
        {
            return
                $"OTP:{purpose}:{email.ToLowerInvariant()}";
        }


        public string GetResetTokenCacheKey(
            string email)
        {
            return
                $"ResetToken:{email.ToLowerInvariant()}";
        }


        public  void ValidateOtpFormat(
            string otp)
        {
            if (string.IsNullOrWhiteSpace(otp) ||
                otp.Length != 6 ||
                !otp.All(char.IsDigit))
            {
                throw new Exception(
                    "OTP must be 6 digits.");
            }
        }



        public  string HashValue(
            string value)
        {
            using var sha256 =
                SHA256.Create();


            var bytes =
                Encoding.UTF8.GetBytes(value);


            var hash =
                sha256.ComputeHash(bytes);


            return Convert.ToBase64String(
                hash);
        }
    }
}
