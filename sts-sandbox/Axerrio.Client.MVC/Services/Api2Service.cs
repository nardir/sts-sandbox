using Axerrio.HttpClientExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.Client.MVC.Services
{
    public class Api2Service : IApi2Service
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private IHttpContextAccessor _httpContextAccesor;

        public Api2Service(IHttpClient apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _httpContextAccesor = httpContextAccessor;

            _remoteServiceBaseUrl = "http://localhost:59160";
        }

        public async Task<string> GetValuesAsync()
        {
            var token = await GetUserTokenAsync();
            var uri = $"{_remoteServiceBaseUrl}/api/values";

            var dataString = await _apiClient.GetStringAsync(uri, token);

            return dataString;
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}
