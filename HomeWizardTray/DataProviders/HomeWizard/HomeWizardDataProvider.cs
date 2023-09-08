using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeWizardTray.DataProviders.HomeWizard
{
    internal sealed partial class HomeWizardDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly string _dataUrl;

        public HomeWizardDataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _dataUrl = $"http://{_appSettings.HomeWizardIpAddress}/api/v1/data";
        }

        public async Task<int> GetActivePower()
        {
            var dataResponseJson = await _httpClient.GetStringAsync(_dataUrl);
            var dataResponse = JsonConvert.DeserializeObject<DataResponse>(dataResponseJson);
            return dataResponse.ActivePower;
        }
    }
}
