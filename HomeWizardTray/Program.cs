using System;
using Serilog;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using HomeWizardTray.DataProviders.SunnyBoy;
using HomeWizardTray.DataProviders.HomeWizard;

namespace HomeWizardTray
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();

            try
            {
                Log.Information("Building app config.");

                var config = new ConfigurationBuilder()
                    .AddJsonFile("appSettings.json", false)
                    .Build();

                var host = Host
                    .CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddMemoryCache();
                        services.AddSingleton<AppSettings>();
                        services.AddHttpClient<HomeWizardDataProvider>();

                        services
                            .AddHttpClient<SunnyBoyDataProvider>()
                            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                            {
                                // Accept broken or lacking certificate from SMA
                                // TODO move and make it specific to the sunnyboy data provider class
                                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                            });

                        services.AddTransient<App>();
                    })
                    .ConfigureAppConfiguration((ctx, builder) =>
                    {
                        Log.Information($"Configuring app with {ctx.HostingEnvironment.EnvironmentName} environment.");

                        if (ctx.HostingEnvironment.IsDevelopment())
                        {
                            builder.AddUserSecrets<App>();
                        }
                    })
                    .Build();

                Log.Information("Running app.");


                var app = host.Services.GetRequiredService<App>();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageBox.Show("An error has occured. Please see log file.", "SunnyTray", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}