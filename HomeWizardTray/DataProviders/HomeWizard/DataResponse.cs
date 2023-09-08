using Newtonsoft.Json;

namespace HomeWizardTray.DataProviders.HomeWizard
{
    internal sealed class DataResponse
    {
        [JsonProperty("active_power_w")]
        public int ActivePower { get; set; }
    }
}
