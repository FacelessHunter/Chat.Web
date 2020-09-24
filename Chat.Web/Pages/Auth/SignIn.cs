using Chat.Web.Domain.Abstractions.Auth;
using Chat.Web.Domain.DTOs.AuthDTOs;
using Chat.Web.Infrastructure.Services.AuthServices;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Web.Pages.Auth
{
    public partial class SignIn
    {
        protected override async Task OnInitializedAsync()
        {
            SignInDTO = new SignInDTO();
        }

        public SignInDTO SignInDTO { get; set; }
        [Inject]
        public IAuthService AuthService { get; set; }

        public async Task HandleOnValidSubmit()
        {
            await AuthService.SignInAsync(SignInDTO);
        }

    }
}
