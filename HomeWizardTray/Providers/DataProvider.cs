using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeWizardTray.Providers
{
    internal sealed class DataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public DataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
        }

        public async Task<DataResponse> GetData()
        {
            var data = await _httpClient.GetStringAsync($"http://{_appSettings.IpAddress}/api/v1/data");
            var response = JsonConvert.DeserializeObject<DataResponse>(data);
            return response;
        }

        public class DataResponse
        {
            [JsonProperty("active_power_w")]
            public int ActivePower { get; set; }
        }
    }
}
