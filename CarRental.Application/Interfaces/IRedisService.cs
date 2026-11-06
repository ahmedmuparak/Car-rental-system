using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IRedisService
    {
        Task<long> IncrementAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
        Task<TimeSpan?> GetTtlAsync(string key);
    }
}
