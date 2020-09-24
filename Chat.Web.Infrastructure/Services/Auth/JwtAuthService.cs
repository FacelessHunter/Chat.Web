using Chat.Web.Domain.Abstractions;
using Chat.Web.Domain.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using Chat.Web.Domain.Abstractions.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Options;
using Chat.Web.Domain.Options;
using System.Security.Claims;

namespace Chat.Web.Infrastructure.Services.AuthServices
{
    public class JwtAuthService : IAuthService
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly JwtAuthStateProvider _jwtAuthStateProvider;
        private readonly IOptions<AuthOptions> _options;

        public JwtAuthService(ITokenProvider tokenProvider, AuthenticationStateProvider authenticationStateProvider, IOptions<AuthOptions> options)
        {
            _tokenProvider = tokenProvider;
            _jwtAuthStateProvider = authenticationStateProvider as JwtAuthStateProvider;
            _options = options;
        }

        public async Task<bool> SignInAsync(SignInDTO signInDTO)
        {
            var tokenPair = await _tokenProvider.GetNewTokensAsync(signInDTO.Username, signInDTO.Password);

            await _jwtAuthStateProvider.SetTokensToStorageAsync(tokenPair);

            if(!await _tokenProvider.ValidateTokenAsync(tokenPair.AccesToken))
            {
                return false;
            }
            var claims = _tokenProvider.ParseToken(tokenPair.AccesToken);

            _jwtAuthStateProvider.MarkUserAsAuthenticated(claims);
            return true;
        }

        public async Task<bool> SignOutAsync()
        {
            return await _jwtAuthStateProvider.RemoveTokensInStorageAsync();
        }

        public async Task<bool> SignUpAsync(SignUpDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenPair> GetTokensAsync()
        {
            if (_jwtAuthStateProvider.Tokens == null)
            {

            }
            return await GetTokensFromStorageAsync();
        }

     
    }
}
