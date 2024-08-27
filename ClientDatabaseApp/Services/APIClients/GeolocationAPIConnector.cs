using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Services.APIClients //Requests -> nolimit
{
    public interface IGeolocationAPIConnector
    {
        string Lat { get; }
        string Lon { get; }
        string City { get; }
        Task GetCoorAsync(string ipAddress);
    }
    public class GeolocationAPIConnector : IGeolocationAPIConnector
    {
        public string Lat { get; private set; }
        public string Lon { get; private set; }
        public string City { get; private set; }

        private readonly HttpClient _httpClient;

        public GeolocationAPIConnector(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task GetCoorAsync(string ipAddress)
        {
            string requestUri = $"http://ip-api.com/json/{ipAddress}?fields=status,message,country,regionName,city,zip,lat,lon,query&lang=pl";
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                dynamic locationData = JsonConvert.DeserializeObject(content);
                Lat = locationData.lat;
                Lon = locationData.lon;
                City = locationData.city;
            }
        }


    }

}
