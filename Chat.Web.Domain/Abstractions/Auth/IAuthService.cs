using Chat.Web.Domain.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Domain.Abstractions.Auth
{
    public interface IAuthService
    {

        Task<bool> SignInAsync(SignInDTO loginDTO);

        Task<bool> SignUpAsync(SignUpDTO loginDTO);

        Task SignOutAsync();
    }
}
