using Newtonsoft.Json;

namespace HomeWizardTray.DataProviders.HomeWizard
{
    internal sealed partial class HomeWizardDataProvider
    {
        private class DataResponse
        {
            [JsonProperty("active_power_w")]
            public int ActivePower { get; set; }
        }
    }
}
