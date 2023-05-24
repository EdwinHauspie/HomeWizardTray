using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.DataProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace HomeWizardTray
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();

            var configBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false);
            configBuilder.Build();

            var hostBuilder = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<AppSettings>();

                    services.AddHttpClient<HomeWizardDataProvider>();

                    services
                        .AddHttpClient<SunnyBoyDataProvider>()
                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        });

                    services.AddTransient<App>();
                });

            var host = hostBuilder.Build();
            var app = host.Services.GetRequiredService<App>();

            Application.Run(app);
        }
    }
}