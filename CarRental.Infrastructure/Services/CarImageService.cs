using CarRental.Application.Configuration;
using CarRental.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    public class CarImageService : ICarImageService
    {
        private readonly HttpClient _httpClient;
        private readonly CarImagesOptions _options;

        public CarImageService(
            HttpClient httpClient,
            IOptions<CarImagesOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string?> GetCarImageUrlAsync(
            string make,
            string model,
            int year,
            string? view = null)
        {
            var query =
                $"api_key={Uri.EscapeDataString(_options.ApiKey)}" +
                $"&api_secret={Uri.EscapeDataString(_options.ApiSecret)}" +
                $"&make={Uri.EscapeDataString(make)}" +
                $"&model={Uri.EscapeDataString(model)}" +
                $"&year={year}" +
                $"&width=800";

            if (!string.IsNullOrWhiteSpace(view))
            {
                query += $"&view={Uri.EscapeDataString(view)}";
            }

            var url =
                $"{_options.BaseUrl}/api/v1/signed-url?{query}";

            var response =
                await _httpClient.GetFromJsonAsync<SignedUrlResponse>(url);

            return response?.Url;
        }

        private class SignedUrlResponse
        {
            public string Url { get; set; } = string.Empty;
        }
    }
}
