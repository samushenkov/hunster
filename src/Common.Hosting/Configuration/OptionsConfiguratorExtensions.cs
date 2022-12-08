using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Hosting.Configuration
{
    public static class OptionsConfiguratorExtensions
    {
        public static void ConfigureByName<TOptions>(this IServiceCollection services)
            where TOptions : class, new()
        {
            services.AddOptions();
            services.AddTransient<IConfigureOptions<TOptions>, OptionsConfigurator<TOptions>>();
        }
    }
}
