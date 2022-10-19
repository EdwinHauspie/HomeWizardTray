using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using HomeWizardTray.Extensions;

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
                error = "Invalid IP address provided.";
                return false;
            }

            //TODO check if ip is the P1 meter

            if (Thresholds.Length != 5 || Thresholds.Any(t => t <= 0))
            {
                error = "Invalid thresholds provided.";
                return false;
            }

            if (!Color.IsValidHexColor())
            {
                error = "Invalid hexadecimal color.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
