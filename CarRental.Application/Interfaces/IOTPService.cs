using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IOTPService
    {
        public  string HashValue(string value);
        public void SaveOtp(string email,string otp,OtpPurpose purpose);
        public bool ValidateOtp(string email,string otp,OtpPurpose purpose);
        public void RemoveOtp(string email,OtpPurpose purpose);
        public string GetOtpCacheKey(string email,OtpPurpose purpose);
        public string GetResetTokenCacheKey(string email);
        public void ValidateOtpFormat(string otp);
        public  string GenerateOtp();
    }
}
