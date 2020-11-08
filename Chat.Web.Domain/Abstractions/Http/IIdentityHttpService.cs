using Common.Domain.DTOs.ChatDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Domain.Abstractions.Http
{
    public interface IIdentityHttpService
    {
        Task<ViewChatDTO[]> GetUserChats();
    }
}
