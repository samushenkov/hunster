using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Hosting.Configuration
{
    public class OptionsConfigurator<TOptions> : IConfigureOptions<TOptions>
        where TOptions : class, new()
    {
        private readonly IConfiguration _configuration;

        public OptionsConfigurator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(TOptions options)
        {
            var sectionName = typeof(TOptions).Name;

            var configurationSection = _configuration.GetSection(sectionName);
            var configurationSectionExists = configurationSection.Exists();

            if (configurationSectionExists)
            {
                configurationSection.Bind(options);
            }
        }
    }
}
