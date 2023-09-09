using System;
using Log = Serilog.Log;
using Microsoft.Extensions.Configuration;
using HomeWizardTray.DataProviders.Sma;

namespace HomeWizardTray
{
    internal sealed class AppSettings
    {
        public AppSettings(IConfiguration config)
        {
            try
            {
                config.Bind(this);

                if (string.IsNullOrWhiteSpace(SunnyBoyPass))
                {
                    throw new Exception($"{nameof(SunnyBoyPass)} can not be null or empty. Please check the app settings file.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public ushort UpdateIntervalSeconds { get; set; } = 3;
        public string Ftxm25IpAddress {get; set;}
        public string P1MeterIpAddress { get; set; }
        public string SunnyBoyIpAddress { get; set; }
        public User SunnyBoyUser { get; set; }
        public string SunnyBoyPass { get; set; }
    }
}
