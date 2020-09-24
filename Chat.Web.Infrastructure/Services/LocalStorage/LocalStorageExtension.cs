using Chat.Web.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Web.Infrastructure.Services.LocalStorage
{
    public static class LocalStorageExtension
    {
        public static IServiceCollection AddLocalStorageService(this IServiceCollection services)
        {
            return services
                .AddScoped<ILocalStorageService, LocalStorageService>();
        }
    }
}
