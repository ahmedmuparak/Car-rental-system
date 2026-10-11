using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Domain.Enums
{
    public enum NotificationType
    {
        Booking = 1,
        BookingConfirmed = 2,
        BookingCancelled = 3,
        BookingCompleted = 4,

        Payment = 5,
        PaymentSuccess = 6,
        PaymentFailed = 7,

        Car = 8,
        CarAvailable = 9,
        CarUnavailable = 10,

        Message = 11,

        System = 12
    }
}
