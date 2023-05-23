using System;
using Log = Serilog.Log;
using Microsoft.Extensions.Configuration;

namespace HomeWizardTray
{
    internal class AppSettings
    {
        public AppSettings(IConfiguration config)
        {
            try
            {
                config.Bind(this);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public string Format { get; set; } = "{0}W";
        public ushort UpdateIntervalSeconds { get; set; } = 3;

        public string HomeWizardIpAddress { get; set; } = "127.0.0.1";
        public string SunnyBoyIpAddress { get; set; } = "127.0.0.1";
    }
}
