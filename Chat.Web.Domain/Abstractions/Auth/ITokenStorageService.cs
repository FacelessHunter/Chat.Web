using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Domain.Abstractions.Auth
{
    public interface ITokenStorageService
    {
        Task SetTokensAsync(TokenPair tokenPair);

        Task<string> GetTokenAsync();

        Task<bool> RemoveTokensAsync();
    }
}
