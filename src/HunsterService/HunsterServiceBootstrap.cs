using Common.Hosting.Configuration;
using HunsterService.Cors;
using HunsterService.MatchTracker;
using HunsterService.Swagger;
using Hunt;
using Hunt.GameFolder;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace HunsterService
{
    internal class HunsterServiceBootstrap
    {
        static async Task Main(string[] args)
        {
            try
            {
                var hostBuilder = new HostBuilder();

                hostBuilder.ConfigureWebHost(webHostBuilder =>
                {
                    ConfigureHost(hostBuilder, args);
                    ConfigureWebHost(webHostBuilder);
                });

                using (var host = hostBuilder.Build())
                {
                    var hostLogger = host.Services.GetRequiredService<ILogger<HunsterServiceBootstrap>>();
                    var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

                    hostLogger.LogInformation("Service [{appName}] started in [{envName}] environment",
                        hostEnvironment.ApplicationName,
                        hostEnvironment.EnvironmentName
                    );

                    try
                    {
                        await host.StartAsync();
                        await host.WaitForShutdownAsync();
                    }
                    catch (Exception ex)
                    {
                        hostLogger.LogCritical(ex, "Program stopped with error");
                    }
                    finally
                    {
                        hostLogger.LogInformation("Service stopped");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Write(ex.StackTrace);
            }
        }

        static void ConfigureHost(IHostBuilder hostBuilder, string[] args)
        {
            hostBuilder.ConfigureHostConfiguration(builder =>
            {
                // Environment configuration
                builder.AddEnvironmentVariables();

                // File configuration
                builder.AddJsonFile("HunsterService.json", true);
            });

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    // Load configuration from logging section
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));

                    // Register loggers
                    builder.AddConsole();
                    builder.AddEventLog();
                });

                // Configure generic services
                ConfigureServices(context, services);
            });

            hostBuilder.UseWindowsService();
        }

        static void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            // Use Kestrel to handle requests
            webHostBuilder.UseKestrel((context, options) =>
                options.Configure(context.Configuration.GetSection("Kestrel"), reloadOnChange: true)
            );

            // Configure web host services
            webHostBuilder.ConfigureServices(ConfigureWebServices);

            // Configure application
            webHostBuilder.Configure(ConfigureWebApplication);

            // Custom configuration
            webHostBuilder.UseSetting(WebHostDefaults.ApplicationKey, null);
            webHostBuilder.UseSetting(WebHostDefaults.PreventHostingStartupKey, "true");
            webHostBuilder.UseSetting(WebHostDefaults.ServerUrlsKey, "");
            webHostBuilder.UseSetting(WebHostDefaults.WebRootKey, "Web");
        }

        static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            #region [GameFolderMatchTracker]
            services.ConfigureByName<GameFolderMatchTrackerOptions>();
            services.AddSingleton<GameFolderMatchTracker>();
            services.AddTransient<IMatchProvider>(p => p.GetRequiredService<GameFolderMatchTracker>());
            services.AddTransient<IMatchEmitter>(p => p.GetRequiredService<GameFolderMatchTracker>());
            #endregion

            #region [MatchTrackerService]
            services.ConfigureByName<MatchTrackerServiceOptions>();
            services.AddHostedService<MatchTrackerService>();
            #endregion
        }

        static void ConfigureWebServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();

            services.Configure<CorsOptions>(options =>
            {
                options.AddPolicy(CorsPolicies.HUNSTER_ENDPOINT, policyBuilder =>
                {
                    // allow development origin
                    policyBuilder.WithOrigins("http://localhost:3000");
                });
            });
        }

        static void ConfigureWebApplication(WebHostBuilderContext context, IApplicationBuilder builder)
        {
            builder.UseStaticFiles();
            builder.UseSwaggerByRoute();

            // Use routing
            builder.UseRouting();

            builder.UseCors();

            // Configure endpoints routing
            builder.UseEndpoints(builder =>
            {
                builder.MapControllers();
                builder.MapFallbackToFile("index.html");
            });
        }
    }
}