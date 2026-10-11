using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.DTOs.Payment
{
    public class StripeSettingsDTO
    {
        public string SecretKey { get; set; } = string.Empty;

        public string PublishableKey { get; set; } = string.Empty;

        public string WebhookSecret { get; set; } = string.Empty;

        public string SuccessUrl { get; set; } = string.Empty;

        public string CancelUrl { get; set; } = string.Empty;
    }
}
