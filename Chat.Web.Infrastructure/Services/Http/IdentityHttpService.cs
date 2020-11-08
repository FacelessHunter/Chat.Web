using Chat.Web.Domain.Abstractions.Auth;
using Chat.Web.Domain.Abstractions.Http;
using Common.Domain.DTOs;
using Common.Domain.DTOs.ChatDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Infrastructure.Services.Http
{
    public class IdentityHttpService : HttpService, IIdentityHttpService
    {
        public IdentityHttpService(HttpClient httpClient, ITokenStorageService tokenStorageService)
            : base(httpClient, tokenStorageService)
        {
        }

        public async Task<ViewChatDTO[]> GetUserChats()
        {
            var temp = await GetAsync<ContentDTO<ViewChatDTO>>("api/chats").ConfigureAwait(false);
            Console.WriteLine(temp.TotalCount);
            return temp.Content;
        }
    }
}
