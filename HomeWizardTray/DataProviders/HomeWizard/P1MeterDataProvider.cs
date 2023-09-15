using HomeWizardTray.DataProviders.HomeWizard.Dto;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeWizardTray.DataProviders.HomeWizard
{
    internal sealed class P1MeterDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly string _baseUrl;

        public P1MeterDataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _baseUrl = $"http://{_appSettings.P1MeterIpAddress}";
        }

        public async Task<int> GetActivePower()
        {
            var dataResponseJson = await _httpClient.GetStringAsync($"{_baseUrl}/api/v1/data");
            var dataResponse = JsonConvert.DeserializeObject<DataResponse>(dataResponseJson);
            return dataResponse.ActivePower;
        }
    }
}
