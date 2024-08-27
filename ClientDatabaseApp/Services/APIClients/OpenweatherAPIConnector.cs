using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Services.APIClients
{
    public interface IOpenweatherAPIConnector
    {
        double Temperature { get; }
        double TemperatureFeel { get; }
        double Wind { get; }
        string Weather { get; }
        Task GetWeatherAsync(string lat, string lon);
    }

    public class OpenweatherAPIConnector : IOpenweatherAPIConnector //Requests -> 60 calls/minute  1,000,000 calls/month
    {

        public double Temperature { get; set; }
        public double TemperatureFeel { get; set; }
        public double Wind { get; set; }
        public string Weather { get; set; }

        private readonly HttpClient _httpClient;

        public OpenweatherAPIConnector(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task GetWeatherAsync(string lat, string lon)
        {
            string accessKey = ConfigurationManager.AppSettings["AccessKey"];
            string requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={accessKey}&units=metric&lang=pl";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                dynamic weatherData = JsonConvert.DeserializeObject(content);
                Temperature = weatherData.main.temp;
                TemperatureFeel = weatherData.main.feels_like;
                Wind = weatherData.wind.speed;
                Weather = weatherData.weather[0].description;
            }

        }


    }
}
