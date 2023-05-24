using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HomeWizardTray.DataProviders
{
    internal sealed class SunnyBoyDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly string _baseUrl;
        private string _sid;

        public SunnyBoyDataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _baseUrl = $"https://{_appSettings.SunnyBoyIpAddress}";
        }

        private async Task Login()
        {
            var postData = new Dictionary<string, string>
            {
                { "right", _appSettings.SunnyBoyUser },
                { "pass", _appSettings.SunnyBoyPass }
            };

            SetHeaders();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/dyn/login.json", postData);
            _httpClient.DefaultRequestHeaders.Clear();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
            _sid = loginResponse.Result.Sid;
        }

        private class LoginResponse
        {
            public LoginResult Result { get; set; }

            public class LoginResult
            {
                public string Sid { get; set; }
            }
        }

        public async Task<int> GetActivePower()
        {
            if (_sid == null)
            {
                await Login();
            }

            var postData = new Dictionary<string, object>
            {
                { "keys", new List<string> { "6100_40263F00" } },
                { "destDev", new List<object>() }
            };

            SetHeaders();
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/dyn/getValues.json?sid=" + _sid, postData);
            _httpClient.DefaultRequestHeaders.Clear();
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var watt = responseContent.Split(':').Last().Split('}').First();
            return watt == "null" ? 0 : int.Parse(watt);
        }

        private string GetCookie()
        {
            return
                $"tmhDynamicLocale.locale=%22en-us%22; " +
                $"deviceUsr443=%22{_appSettings.SunnyBoyUser}%22; " +
                $"deviceMode443=%22PSK%22; " +
                $"user443=%7B%22role%22%3A%7B%22bitMask%22%3A4%2C%22title%22%3A%22{_appSettings.SunnyBoyUser}%22%2C%22loginLevel%22%3A2%7D%2C%22username%22%3A862%2C%22sid%22%3A%22{_sid}%22%7D; " +
                $"deviceSid443=%22{_sid}%22";
        }

        private void SetHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Host", _appSettings.SunnyBoyIpAddress);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            _httpClient.DefaultRequestHeaders.Add("Referer", $"{_baseUrl}/");
            if (_sid != null) _httpClient.DefaultRequestHeaders.Add("Cookie", GetCookie());
        }
    }
}
