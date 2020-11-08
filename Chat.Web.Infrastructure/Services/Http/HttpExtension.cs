using Chat.Web.Domain.Abstractions.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Chat.Web.Infrastructure.Services.Http
{
    public static class HttpExtension
    {
        public static void AddHttpServices(this IServiceCollection services)
        {
            services.AddHttpClient<IIdentityHttpService, IdentityHttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:6443/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }
    }
}
