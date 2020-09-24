using Chat.Web.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Domain.Abstractions.Auth
{


    public interface ITokenProvider
    {
        Task<TokenPair> GetNewTokensAsync(string username, string password);

        Task<bool> IntrospectTokenAsync(string token, TokenTypeHint tokenType);

        Task<TokenPair> RenewTokensAsync(string refreshToken);

        Task<bool> RevocationTokenAsync(string token, TokenTypeHint tokenType);

        Task<bool> ValidateTokenAsync(string token);

        ClaimsPrincipal ParseToken(string token);



    }
}
