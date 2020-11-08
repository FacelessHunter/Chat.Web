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
     sealed public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly IOptions<AuthOptions> _options;
        private readonly ILogger<JwtAuthStateProvider> _logger;
        public TokenPair Tokens { get; set; }


        public JwtAuthStateProvider(ITokenProvider tokenProvider, ILocalStorageService localStorageService, ITokenStorageService tokenStorageService, IOptions<AuthOptions> options, ILogger<JwtAuthStateProvider> logger)
        {
            _tokenProvider = tokenProvider;
            _localStorageService = localStorageService;
            _tokenStorageService = tokenStorageService;
            _options = options;
            _logger = logger;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            bool isAccessTokenExist = await _localStorageService.ContainKeyAsync(_options.Value.AccessTokenName);
            bool isRefreshTokenExist = await _localStorageService.ContainKeyAsync(_options.Value.RefreshTokenName);

            if (!(isAccessTokenExist || isRefreshTokenExist)) 
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymousUser);
            }

            var token = await _tokenStorageService.GetTokenAsync();

            var claims = _tokenProvider.ParseToken(token);

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
    }
}