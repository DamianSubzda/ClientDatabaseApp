using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Services.APIClients //Requests -> nolimit
{
    public interface IIpifyAPIConnector
    {
        string IPAddress { get; }
        Task GetIpAsync();
    }

    public class IpifyAPIConnector : IIpifyAPIConnector
    {
        public string IPAddress { get; private set; }

        private readonly HttpClient _httpClient;

        public IpifyAPIConnector(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task GetIpAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://api.ipify.org?format=json");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(content);
                IPAddress = data.ip;
            }
        }


    }
}
