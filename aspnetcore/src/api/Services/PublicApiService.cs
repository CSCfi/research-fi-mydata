using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models.Ttv;
using api.Models.Keycloak;
using api.PublicApiContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace api.Services
{
    public class PublicApiService : IPublicApiService
    {
        private readonly TtvContext _ttvContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PublicApiService> _logger;


        public PublicApiService(
            TtvContext ttvContext,
            IHttpClientFactory httpClientFactory,
            ILogger<PublicApiService> logger)
        {
            _ttvContext = ttvContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Mockup: build greeting response. Real DimUserProfile-derived data is a future phase.
        public PublicApiHelloResponse GetHelloMessage(string username)
        {
            return new PublicApiHelloResponse { Message = $"Hello {username}" };
        }

        public string GetUsernameFromNationalIdentificationNumber(string nationalIdentificationNumber)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(nationalIdentificationNumber.ToUpper());
            byte[] hashBytes = MD5.HashData(inputBytes);
            StringBuilder sb = new StringBuilder(2 * hashBytes.Length);
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}