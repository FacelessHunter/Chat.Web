using Chat.Web.Domain.Abstractions;
using Chat.Web.Domain.Abstractions.Auth;
using Chat.Web.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Infrastructure.Services.Auth
{
    public class JwtTokenStorageService : ITokenStorageService
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IOptions<AuthOptions> _options;
        private readonly ILogger<JwtTokenStorageService> _logger;

        private TokenPair tokens;

        public JwtTokenStorageService(ITokenProvider tokenProvider, ILocalStorageService localStorageService, IOptions<AuthOptions> options, ILogger<JwtTokenStorageService> logger)
        {
            _tokenProvider = tokenProvider;
            _localStorageService = localStorageService;
            _options = options;
            _logger = logger;
        }

        public async Task SetTokensAsync(TokenPair tokenPair)
        {
            await _localStorageService.SetItemAsync<string>(_options.Value.AccessTokenName, tokenPair.AccessToken);
            await _localStorageService.SetItemAsync<string>(_options.Value.RefreshTokenName, tokenPair.RefreshToken);
            tokens = new TokenPair(tokenPair.AccessToken, tokenPair.RefreshToken);
        }

        public async Task<string> GetTokenAsync()
        {
            TokenPair tokenPair = new TokenPair("", "");

            if (tokens != null)
            {
                tokenPair = tokens;
            }
            else if(tokens == null)
            {
                string accessToken = await _localStorageService.GetItemAsync<string>(_options.Value.AccessTokenName).ConfigureAwait(true);
                string refreshToken = await _localStorageService.GetItemAsync<string>(_options.Value.RefreshTokenName).ConfigureAwait(false);
                tokenPair = new TokenPair(accessToken, refreshToken);
            }

            if (String.IsNullOrWhiteSpace(tokenPair.AccessToken) || String.IsNullOrWhiteSpace(tokenPair.RefreshToken))
                throw new Exception("Invalid token");

            if (!await _tokenProvider.ValidateTokenAsync(tokenPair.AccessToken))
            {
                tokenPair = await _tokenProvider.RenewTokensAsync(tokenPair.RefreshToken);
                await SetTokensAsync(tokenPair);
            }
            tokens = tokenPair;
            return tokenPair.AccessToken;
        }

        public async Task<bool> RemoveTokensAsync()
        {
            await _localStorageService.RemoveItemAsync(_options.Value.AccessTokenName);
            await _localStorageService.RemoveItemAsync(_options.Value.RefreshTokenName);

            if (!(await _localStorageService.ContainKeyAsync(_options.Value.AccessTokenName)
                && await _localStorageService.ContainKeyAsync(_options.Value.RefreshTokenName)))
                return false;
            return true;
        }
    }
}
