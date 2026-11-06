using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRental.Application.Interfaces;
namespace CarRental.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly StackExchange.Redis.IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<long> IncrementAsync(string key)
        {
            return await _database.StringIncrementAsync(key);
        }

        public async Task<bool> SetExpiryAsync(
            string key,
            TimeSpan expiry)
        {
            return await _database.KeyExpireAsync(key, expiry);
        }
        public async Task<TimeSpan?> GetTtlAsync(string key)
        {
            return await _database.KeyTimeToLiveAsync(key);
        }
    }
}
