using Chat.Web.Domain.Abstractions;
using Chat.Web.Domain.Abstractions.Auth;
using Chat.Web.Domain.Options;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;


namespace Chat.Web
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IOptions<AuthOptions> _options;
        private readonly ILogger<JwtAuthStateProvider> _logger;
        public TokenPair Tokens { get; set; }


        public JwtAuthStateProvider(ITokenProvider tokenProvider, ILocalStorageService localStorageService, IOptions<AuthOptions> options, ILogger<JwtAuthStateProvider> logger)
        {
            _tokenProvider = tokenProvider;
            _localStorageService = localStorageService;
            _options = options;
            _logger = logger;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenPair = await GetTokensFromStorageAsync();


            if (String.IsNullOrWhiteSpace(tokenPair.AccesToken) || String.IsNullOrWhiteSpace(tokenPair.RefreshToken))
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymousUser);
            }

            if (!await _tokenProvider.ValidateTokenAsync(tokenPair.AccesToken))
            {
                tokenPair = await _tokenProvider.RenewTokensAsync(tokenPair.RefreshToken);

                await SetTokensToStorageAsync(tokenPair);
            }

            var claims = _tokenProvider.ParseToken(tokenPair.AccesToken);

            var user = new ClaimsPrincipal(claims);

            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated(ClaimsPrincipal claims)
        {
            var authState = Task.FromResult(new AuthenticationState(claims));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task SetTokensToStorageAsync(TokenPair tokenPair)
        {
            await _localStorageService.SetItemAsync<string>(_options.Value.AccessTokenName, tokenPair.AccesToken);
            await _localStorageService.SetItemAsync<string>(_options.Value.RefreshTokenName, tokenPair.RefreshToken);
            Tokens = new TokenPair(tokenPair.AccesToken, tokenPair.RefreshToken);
        }

        public async Task<TokenPair> GetTokensFromStorageAsync()
        {
            string accessToken = await _localStorageService.GetItemAsync<string>(_options.Value.AccessTokenName);
            string refreshToken = await _localStorageService.GetItemAsync<string>(_options.Value.RefreshTokenName);

            return new TokenPair(accessToken, refreshToken);
        }

        public async Task<bool> RemoveTokensInStorageAsync()
        {
            await _localStorageService.RemoveItemAsync(_options.Value.AccessTokenName);
            await _localStorageService.RemoveItemAsync(_options.Value.RefreshTokenName);

            if (!(await _localStorageService.ContainKeyAsync(_options.Value.AccessTokenName)
                && await _localStorageService.ContainKeyAsync(_options.Value.RefreshTokenName)))
                return false;
            return true;
        }
        private async Task<TokenPair> Validate()
        {

            throw new NotImplementedException();
        }
    }
}