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

        public string Format { get; set; }
        public string IpAddress { get; set; }
        public int UpdateInterval { get; set; }

        public bool Validate(out string error)
        {
            if (!IPAddress.TryParse(IpAddress, out var _))
            {
                error = "Invalid IP address.";
                return false;
            }

            if (UpdateInterval < 100)
            {
                error = "Invalid udate interval.";
                return false;
            }

            //TODO check if ip is the HW P1 meter

            error = null;
            return true;
        }
    }
}
