using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;

namespace HomeWizardTray.DataProviders.Daikin
{
    internal sealed class Ftxm25DataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly string _baseUrl;

        public Ftxm25DataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _baseUrl = $"http://{_appSettings.Ftxm25IpAddress}";
        }

        public async Task<Dictionary<string, string>> GetInfo()
        {
            var dataResponse = await _httpClient.GetStringAsync($"{_baseUrl}/aircon/get_control_info");
            var toQueryString = dataResponse.Replace(",", "&");
            var kvs = HttpUtility.ParseQueryString(toQueryString);
            return kvs.Cast<string>().ToDictionary(k => k, v => kvs[v]);
        }

        public async Task SetMax()
        {
            await SetControlInfo(new Dictionary<string, string>
            {
                [Keys.Power] = Power.On,
                [Keys.Mode] = Mode.Cool,
                [Keys.Temperature] = "18.0",
                [Keys.Humidity] = "0",
                [Keys.FanSpeed] = FanSpeed.Level5,
                [Keys.FanMotion] = FanMotion.None,
            });

            await SetSpecialMode(new Dictionary<string, string>
            {
                [Keys.SpecialModeState] = SpecialModeState.Off,
                [Keys.SpecialMode] = SpecialMode.Econo
            });
        }

        public async Task SetLevel2()
        {
            await SetControlInfo(new Dictionary<string, string>
            {
                [Keys.Power] = Power.On,
                [Keys.Mode] = Mode.Cool,
                [Keys.Temperature] = "18.0",
                [Keys.Humidity] = "0",
                [Keys.FanSpeed] = FanSpeed.Level2,
                [Keys.FanMotion] = FanMotion.None,
            });

            await SetSpecialMode(new Dictionary<string, string>
            {
                [Keys.SpecialMode] = SpecialMode.Econo,
                [Keys.SpecialModeState] = SpecialModeState.Off,
            });
        }

        public async Task SetEco()
        {
            await SetControlInfo(new Dictionary<string, string>
            {
                [Keys.Power] = Power.On,
                [Keys.Mode] = Mode.Cool,
                [Keys.Temperature] = "18.0",
                [Keys.Humidity] = "0",
                [Keys.FanSpeed] = FanSpeed.Silent,
                [Keys.FanMotion] = FanMotion.None,
            });

            await SetSpecialMode(new Dictionary<string, string>
            {
                [Keys.SpecialMode] = SpecialMode.Econo,
                [Keys.SpecialModeState] = SpecialModeState.On,
            });
        }

        private async Task SetControlInfo(Dictionary<string, string> dic)
        {
            var queryString = string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var response = await _httpClient.PostAsync($"{_baseUrl}/aircon/set_control_info?{queryString}", null);
            var resonseBody = await response.Content.ReadAsStringAsync();
        }

        private async Task SetSpecialMode(Dictionary<string, string> dic)
        {
            var queryString = string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var response = await _httpClient.PostAsync($"{_baseUrl}/aircon/set_special_mode?{queryString}", null);
            var resonseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
