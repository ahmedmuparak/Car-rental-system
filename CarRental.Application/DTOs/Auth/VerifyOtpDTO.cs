using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Auth
{
    public class VerifyOtpDTO
    {
        public string Email { get; set; } = string.Empty;

        public string OTP { get; set; } = string.Empty;
    }
}
