using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSIsoft.Data.Http;
using OSIsoft.Identity;
using OSIsoft.Omf;
using OSIsoft.Omf.Converters;
using OSIsoft.OmfIngress;

namespace OpenWeather
{
    public static class Program
    {
        private static readonly HttpClient _client = new(); 
        private static IOmfIngressService _omfIngressService;
        private static ILogger _log;

        public static AppSettings Settings { get; set; }

        [FunctionName("CurrentWeather")]
#pragma warning disable CA1801 // Review unused parameters; this method declaration follows Azure Function examples
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
#pragma warning restore CA1801 // Review unused parameters
        {
            _log = log;
            LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            LoadConfiguration();

            if (string.IsNullOrEmpty(Settings.OpenWeatherKey))
            {
                LogInformation("No OpenWeather API Key provided, function will generate random data");
            }

            // Set up OMF Ingress Service
            _omfIngressService = ConfigureAdhOmf(Settings.AdhUri, Settings.AdhTenantId, Settings.AdhNamespaceId, Settings.AdhClientId, Settings.AdhClientSecret);

            // Send OMF Type message
            SendOmfMessage(_omfIngressService, OmfMessageCreator.CreateTypeMessage(typeof(CurrentWeather)));

            // Prepare OMF containers
            string typeId = ClrToOmfTypeConverter.Convert(typeof(CurrentWeather)).Id;
            List<OmfContainer> containers = new();
            Dictionary<string, IEnumerable<CurrentWeather>> data = new();

            string[] queries = Settings.OpenWeatherQueries.Split('|');
            foreach (string query in queries)
            {
                if (!string.IsNullOrEmpty(Settings.OpenWeatherKey))
                {
                    // Get Current Weather Data
                    JObject response = JsonConvert.DeserializeObject<JObject>(HttpGet($"{Settings.OpenWeatherUri}?q={query}&appid={Settings.OpenWeatherKey}"));

                    // Parse data into OMF messages
                    CurrentWeather value = new(response);
                    string streamId = $"OpenWeather_Current_{value.Name}";
                    containers.Add(new OmfContainer(streamId, typeId));
                    data.Add(streamId, new CurrentWeather[] { value });
                }
                else
                {
                    // No key provided, generate random data
                    containers.Add(new OmfContainer(query, typeId));
                    CurrentWeather value = new(query);
                    data.Add(query, new CurrentWeather[] { value });
                }
            }

            SendOmfMessage(_omfIngressService, new OmfContainerMessage(containers));
            LogInformation($"Sent {containers.Count} containers");
            SendOmfMessage(_omfIngressService, OmfMessageCreator.CreateDataMessage(data));
            LogInformation($"Sent {data.Count} data messages");
        }

        private static void LogInformation(string message)
        {
            if (_log != null)
            {
                _log.LogInformation(message);
            }
        }

        private static void LoadConfiguration()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/appsettings.json"))
            {
                // Running locally, read configuration from file
                Settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Directory.GetCurrentDirectory() + "/appsettings.json"));
            }
            else
            {
                // Running in Azure Function, read configuration from Environment
                Settings = new AppSettings()
                {
                    OpenWeatherUri = new Uri(Environment.GetEnvironmentVariable("OPEN_WEATHER_URI")),
                    OpenWeatherKey = Environment.GetEnvironmentVariable("OPEN_WEATHER_KEY"),
                    OpenWeatherQueries = Environment.GetEnvironmentVariable("OPEN_WEATHER_QUERIES"),
                    AdhUri = new Uri(Environment.GetEnvironmentVariable("ADH_URI")),
                    AdhTenantId = Environment.GetEnvironmentVariable("ADH_TENANT_ID"),
                    AdhNamespaceId = Environment.GetEnvironmentVariable("ADH_NAMESPACE_ID"),
                    AdhClientId = Environment.GetEnvironmentVariable("ADH_CLIENT_ID"),
                    AdhClientSecret = Environment.GetEnvironmentVariable("ADH_CLIENT_SECRET"),
                };
            }
        }

        /// <summary>
        /// Configure ADH/OMF Services
        /// </summary>
        private static IOmfIngressService ConfigureAdhOmf(Uri address, string tenantId, string namespaceId, string clientId, string clientSecret)
        {
            AuthenticationHandler deviceAuthenticationHandler = new(address, clientId, clientSecret);
            OmfIngressService deviceBaseOmfIngressService = new(address, HttpCompressionMethod.None, deviceAuthenticationHandler);
            return deviceBaseOmfIngressService.GetOmfIngressService(tenantId, namespaceId);
        }

        /// <summary>
        /// Runs a generic HTTP GET request against the GitHub API
        /// </summary>
        private static string HttpGet(string url)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, url);
            return Send(request).Result;
        }

        /// <summary>
        /// Send message using HttpRequestMessage
        /// </summary>
        private static async Task<string> Send(HttpRequestMessage request)
        {
            HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false);

            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error sending OMF response code:{response.StatusCode}.  Response {responseString}");
            return responseString;
        }

        /// <summary>
        /// Sends a message to the ADH OMF endpoint
        /// </summary>
        private static object SendOmfMessage(IOmfIngressService service, OmfMessage omfMessage)
        {
            SerializedOmfMessage serializedOmfMessage = OmfMessageSerializer.Serialize(omfMessage);
            return service.SendOmfMessageAsync(serializedOmfMessage).Result;
        }
    }
}
