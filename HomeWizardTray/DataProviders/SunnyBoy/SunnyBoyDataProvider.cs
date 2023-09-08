using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace HomeWizardTray.DataProviders.SunnyBoy
{
    internal sealed partial class SunnyBoyDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly string _baseUrl;
        private IMemoryCache _cache;

        public SunnyBoyDataProvider(HttpClient httpClient, IMemoryCache cache, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _cache = cache;
            _appSettings = appSettings;
            _baseUrl = $"https://{_appSettings.SunnyBoyIpAddress}";
        }

        private async Task<string> Login()
        {
            var postData = new Dictionary<string, string>
            {
                { "right", _appSettings.SunnyBoyUsername },
                { "pass", _appSettings.SunnyBoyPassword }
            };

            ClearAndSetDefaultHeaders();

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/dyn/login.json", postData);
            _httpClient.DefaultRequestHeaders.Clear();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().Result;

            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
            var sid = loginResponse?.Result?.Sid;
            if (sid == null) throw new Exception($"Could not log in and retrieve sid from Sunny Boy. Response was: {responseContent}");

            _cache.Set("sid", sid, TimeSpan.FromSeconds(1800));
            return sid;
        }

        public async Task<int> GetActivePower()
        {
            if (!_cache.TryGetValue("sid", out string sid))
            {
                sid = await Login();
            }

            ClearAndSetDefaultHeaders();

            _httpClient.DefaultRequestHeaders.Add("Cookie", 
                $"tmhDynamicLocale.locale=%22en-us%22; " +
                $"deviceUsr443=%22{_appSettings.SunnyBoyUsername}%22; " +
                $"deviceMode443=%22PSK%22; " +
                $"user443=%7B%22role%22%3A%7B%22bitMask%22%3A4%2C%22title%22%3A%22{_appSettings.SunnyBoyUsername}%22%2C%22loginLevel%22%3A2%7D%2C%22username%22%3A862%2C%22sid%22%3A%22{sid}%22%7D; " +
                $"deviceSid443=%22{sid}%22");

            var postData = new Dictionary<string, object>
            {
                { "keys", new List<string> { "6100_40263F00" } },
                { "destDev", new List<object>() }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/dyn/getValues.json?sid={sid}", postData);
            _httpClient.DefaultRequestHeaders.Clear();
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var watt = responseContent.Split(':').Last().Split('}').First();
            return watt == "null" ? 0 : int.Parse(watt);
        }

        private void ClearAndSetDefaultHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Host", _appSettings.SunnyBoyIpAddress);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            _httpClient.DefaultRequestHeaders.Add("Referer", $"{_baseUrl}/");
        }
    }
}
