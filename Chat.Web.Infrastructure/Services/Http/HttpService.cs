using Chat.Web.Domain.Abstractions.Auth;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Infrastructure.Services.Http
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorageService;

        public HttpService(HttpClient httpClient, ITokenStorageService tokenStorageService)
        {
            _httpClient = httpClient;
            _tokenStorageService = tokenStorageService;
        }

        public async Task<TOut> GetAsync<TOut>(string endpoint)
        {
            await SetTokenAsync();

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            var result = await response.Content.ReadAsAsync<TOut>();
            return result;
        }
        public async Task<bool> GetAsync(string endpoint)
        {
            await SetTokenAsync();
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Make http Post request to endpoint. Return TOut  if gave correct type, else return TOut is initialized default values, or return null, if body of http response is empty.
        /// </summary>
        /// <typeparam name="TIn">Type of send object</typeparam>
        /// <typeparam name="TOut">Type of response object</typeparam>
        /// <param name="endpoint">Path of controller</param>
        /// <param name="data">Sending object</param>
        /// <returns>Return response of http request</returns>
        public async Task<TOut> PostAsync<TIn, TOut>(string endpoint, TIn data)
        {
            await SetTokenAsync();

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<TIn>(endpoint, data);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<TOut>();
        }

        public async Task<T> PutAsync<T>(string endpoint, T data)
        {
            await SetTokenAsync();

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<T>(endpoint, data);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task DeleteAsync(string endpoint)
        {
            await SetTokenAsync();
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }

        protected async Task SetTokenAsync()
        {
            string token = await _tokenStorageService.GetTokenAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("HttpService: TokenProvider has returned null or empty token");
            }
            _httpClient.SetBearerToken(token);
        }
    }
}
