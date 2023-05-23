using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace HomeWizardTray.DataProviders
{
    internal sealed class SunnyBoyDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public SunnyBoyDataProvider(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
        }

        //private string ip;
        //private bool use_ssl;
        private string cookie;
        private string lsid;
        //private string ssid;

        public bool Auth()
        {
            this.lsid = __GenSid();

            var parameters = new Dictionary<string, string>
            {
                { "right", __user },
                { "pass", __password }
            };

            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/login.json", new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    if (json.err != null)
                    {
                        Task.Delay(5000).Wait();
                        return Auth();
                    }
                    else
                    {
                        this.ssid = json.result.sid;
                        this.cookie = response.Headers.GetValues("Set-Cookie");
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*public bool Logout()
        {
            var parameters = new Dictionary<string, string>();
            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/logout.json?sid=" + this.ssid, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    if (json.err != null)
                    {
                        return false;
                    }
                    else
                    {
                        this.lsid = null;
                        this.ssid = null;
                        this.cookie = null;
                        return !json.result.isLogin;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }*/

        /*public bool CheckConnection()
        {
            if (this.lsid == null || this.ssid == null || this.cookie == null)
            {
                return false;
            }

            var parameters = new Dictionary<string, string>();
            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/sessionCheck.json?sid=" + this.ssid, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    if (json.result == null || json.result.cntDwnGg == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }*/

        public string GetValue(Key key)
        {
            var parameters = new Dictionary<string, object>
            {
                { "keys", new List<string> { key.tag } },
                { "destDev", new List<object>() }
            };

            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/getValues.json?sid=" + this.ssid, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    if (json.err != null)
                    {
                        return null;
                    }
                    else
                    {
                        this.__serial = ((JObject)json.result).Properties().FirstOrDefault().Name;
                        var val = json.result[this.__serial][key.tag]["1"][0].val;
                        if (val != null)
                        {
                            return val;
                        }
                        else
                        {
                            return "0";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*public dynamic GetAllKeys()
        {
            var parameters = new Dictionary<string, object>
            {
                { "destDev", new List<object>() }
            };

            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/getAllParamValues.json?sid=" + this.ssid, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    this.__serial = ((JObject)json.result).Properties().FirstOrDefault().Name;
                    return json.result[this.__serial];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }*/

        /*public dynamic GetLogger(int start, int end)
        {
            int key = 28672;

            var parameters = new Dictionary<string, object>
            {
                { "destDev", new List<object>() },
                { "key", key },
                { "tEnd", end },
                { "tStart", start }
            };

            var headers = __GetHeader(parameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(this.__url + "/dyn/getLogger.json?sid=" + this.ssid, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json")).Result;
                    response.EnsureSuccessStatusCode();
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(responseContent);
                    this.__serial = ((JObject)json.result).Properties().FirstOrDefault().Name;
                    return json.result[this.__serial];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }*/

        private string __GenSid()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sid = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            return sid;
        }

        private Dictionary<string, string> __GetHeader(Dictionary<string, object> parameters = null)
        {
            var cookie = "";
            if (this.cookie == null)
            {
                cookie = "tmhDynamicLocale.locale=%22en%22; user80=%7B%22role%22%3A%7B%22bitMask%22%3A2%2C%22title%22%3A%22usr%22%2C%22loginLevel % 22 % 3A1 % 7D % 2C % 22username % 22 % 3A861 % 2C % 22sid % 22 % 3A % 22" + this.lsid + " % 22 % 7D";
            }
            var headers = new Dictionary<string, string>
            {
                { "Host", _appSettings.SunnyBoyIpAddress },
                { "User-Agent", "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0" },
                { "Accept", "application/json, text/plain, */*" },
                { "Accept-Language", "fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3" },
                { "Accept-Encoding", "gzip, deflate" },
                { "Referer", this.__url + "/" },
                { "Content-Type", "application/json" },
                { "Content-Length", parameters != null ? parameters.Count.ToString() : "0" },
                { "Cookie", cookie }
            };
            return headers;
        }

        private class Right
        { 
            public static string USER = "usr";
            public static string INSTALLER = "istl";
        }
    }
}
