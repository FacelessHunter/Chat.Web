using Chat.Web.Domain.Abstractions.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Web.Infrastructure.Services.Auth
{
    public static class AuthExtension
    {
        public static void AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<ITokenStorageService, JwtTokenStorageService>();
            services.AddScoped<JwtAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());
            services.AddScoped<IAuthService, JwtAuthService>();
        }
    }
}
