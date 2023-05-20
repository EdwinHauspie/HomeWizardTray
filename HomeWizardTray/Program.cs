using System;
using System.Windows.Forms;
using HomeWizardTray.Providers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeWizardTray
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false);
            configBuilder.Build();

            var hostBuilder = CreateHostBuilder();
            var host = hostBuilder.Build();

            var app = host.Services.GetRequiredService<App>();
            Application.Run(app);
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddSingleton<AppSettings>();
                services.AddHttpClient<DataProvider>();
                services.AddTransient<App>();
            });
        }
    }
}