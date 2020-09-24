using Chat.Web.Domain.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Web.Infrastructure.Services.LocalStorage
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = false
        };

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async ValueTask SetItemAsync<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (data is string)
            {
                await _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, data).ConfigureAwait(false);
            }
            else
            {
                var serialisedData = JsonSerializer.Serialize(data);
                await _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, serialisedData).ConfigureAwait(false);
            }
        }

        public async ValueTask<T> GetItemAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var serialisedData = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serialisedData))
                return default;

            if (serialisedData.StartsWith("{") && serialisedData.EndsWith("}")
                || serialisedData.StartsWith("\"") && serialisedData.EndsWith("\"")
                || typeof(T) != typeof(string))
            {
                return JsonSerializer.Deserialize<T>(serialisedData, jsonSerializerOptions);
            }
            else
            {
                return (T)(object)serialisedData;
            }
        }

        public async ValueTask RemoveItemAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", key).ConfigureAwait(false);
        }

        public async ValueTask ClearAsync()
        {
            await _jSRuntime.InvokeVoidAsync("localStorage.clear").ConfigureAwait(false);
        }

        public async ValueTask<bool> ContainKeyAsync(string key)
        {
            return await _jSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key).ConfigureAwait(false);
        }
    }
}
