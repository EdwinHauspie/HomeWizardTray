using System;
using Log = Serilog.Log;
using Microsoft.Extensions.Configuration;

namespace HomeWizardTray
{
    internal sealed class AppSettings
    {
        public AppSettings(IConfiguration config)
        {
            try
            {
                config.Bind(this);

                if (string.IsNullOrWhiteSpace(SunnyBoyUsername))
                {
                    throw new Exception($"{nameof(SunnyBoyUsername)} can not be null or empty. Please check the app settings file.");
                }

                if (string.IsNullOrWhiteSpace(SunnyBoyPassword))
                {
                    throw new Exception($"{nameof(SunnyBoyPassword)} can not be null or empty. Please check the app settings file.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public ushort UpdateIntervalSeconds { get; set; } = 3;
        public string HomeWizardIpAddress { get; set; } = "127.0.0.1";
        public string SunnyBoyIpAddress { get; set; } = "127.0.0.1";
        public string SunnyBoyUsername { get; set; } = "usr";
        public string SunnyBoyPassword { get; set; } = "";
    }
}
