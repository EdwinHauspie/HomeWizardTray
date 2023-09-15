﻿using Newtonsoft.Json;

namespace HomeWizardTray.DataProviders.HomeWizard.Dto
{
    internal sealed class DataResponse
    {
        [JsonProperty("active_power_w")]
        public int ActivePower { get; set; }
    }
}
