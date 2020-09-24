using Chat.Web.Domain.Abstractions;
using Chat.Web.Domain.Abstractions.Auth;
using Chat.Web.Domain.Enums;
using Chat.Web.Domain.Options;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Infrastructure.Services.Auth
{
    public class TokenProvider : ITokenProvider
    {

        private readonly HttpClient _httpClient;

        private readonly IOptions<AuthOptions> _options;
        private readonly ILogger<TokenProvider> _logger;
        private DiscoveryDocumentResponse _endpoints;

        public TokenProvider(IHttpClientFactory httpClientFactory, IOptions<AuthOptions> options, ILogger<TokenProvider> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(options.Value.AuthServer);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _options = options;
            _logger = logger;
        }

        public async Task<bool> RevocationTokenAsync(string token, TokenTypeHint tokenType)
        {
            var endpoints = await GetDiscoveryDocumentAsync();
            var tokenRevokeResponse = await _httpClient.RevokeTokenAsync(new TokenRevocationRequest() 
            { 
                Address = endpoints.RevocationEndpoint,
                ClientId = _options.Value.ClientId,
                ClientSecret = _options.Value.ClientSecret,
                TokenTypeHint = tokenType.Value,
                Token = token
            });
            if (tokenRevokeResponse.IsError || tokenRevokeResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get New Tokens
        /// </summary>
        /// <returns>Object of <see cref="TokenPair"/>, with acces token and refresh token, or null if <see cref="TokenPair"/> not found</returns>
        public async Task<TokenPair> GetNewTokensAsync(string username, string password)
        {
            string grantType = "password";

            var endpoints = await GetDiscoveryDocumentAsync();
            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = endpoints.TokenEndpoint,
                ClientId = _options.Value.ClientId,
                ClientSecret = _options.Value.ClientSecret,
                Scope = _options.Value.Scope,
                GrantType = grantType,
                UserName = username,
                Password = password, 
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return new TokenPair(tokenResponse.AccessToken, tokenResponse.RefreshToken);
        }

        public async Task<TokenPair> RenewTokensAsync(string refreshToken)
        {
            var endpoints = await GetDiscoveryDocumentAsync();
            var tokenResponse = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                Address = endpoints.TokenEndpoint,
                ClientId = _options.Value.ClientId,
                ClientSecret = _options.Value.ClientSecret,
                RefreshToken = refreshToken
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return new TokenPair(tokenResponse.AccessToken, tokenResponse.RefreshToken);


        }

        public async Task<bool> IntrospectTokenAsync(string token, TokenTypeHint tokenType)
        {
            var endpoints = await GetDiscoveryDocumentAsync();
            var tokenIntrospectResponse = await _httpClient.IntrospectTokenAsync(new TokenIntrospectionRequest()
            {
                Address = endpoints.IntrospectionEndpoint,
                ClientId = _options.Value.ClientId,
                ClientSecret = _options.Value.ClientSecret,
                TokenTypeHint = tokenType.Value,
                Token = token

            }); ;

            if (tokenIntrospectResponse.IsError || tokenIntrospectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var endpoints = await GetDiscoveryDocumentAsync();

            var keys = new List<SecurityKey>();
            foreach (var webKey in endpoints.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = endpoints.Issuer,
                ValidAudience = "identity",
                IssuerSigningKeys = keys,

                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireSignedTokens = true
            };

            var handler = new JwtSecurityTokenHandler();

            try
            {
               handler.ValidateToken(token, parameters, out var _);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }

        public ClaimsPrincipal ParseToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var claims = handler.ReadJwtToken(token).Claims;

                var claimIdentity = new ClaimsIdentity(claims, "jwt");
                var claimPrincipal = new ClaimsPrincipal(claimIdentity);

                return claimPrincipal;
            }
            throw new Exception($"Can not read token: {token}" );
        }

        private async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync()
        {
            if (_endpoints != null)
                return _endpoints;

            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = _httpClient.BaseAddress.ToString(),
                Policy = {
                        RequireHttps = true
                    }
            });

            if (discoveryResponse.IsError)
            {
                throw new Exception(discoveryResponse.Error);
            }

            _endpoints = discoveryResponse;
            return discoveryResponse;

        }

    }
}
