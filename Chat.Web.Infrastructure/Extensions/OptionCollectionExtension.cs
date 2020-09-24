using Chat.Web.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Web.Infrastructure.Extensions
{
    public static class OptionCollectionExtension
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"));


        }
    }
}
