using System.Net;
using Microsoft.Extensions.Configuration;

namespace HomeWizardTray
{
    internal class AppSettings
    {
        public AppSettings(IConfiguration config)
        {
            config.Bind(this);
        }

        public string Color { get; set; }
        public string Format { get; set; }
        public string IpAddress { get; set; }
        public uint[] Thresholds { get; set; }

        public bool Validate(out string error)
        {
            if (!IPAddress.TryParse(IpAddress, out var _))
            {
                error = "Incorrect IP address provided.";
                return false;
            }

            if (Thresholds.Length != 5)
            {
                error = "Incorrect thresholds provided.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
