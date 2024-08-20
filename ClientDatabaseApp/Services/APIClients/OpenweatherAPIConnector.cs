using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Service.API
{
    public class OpenweatherAPIConnector //Requests -> 60 calls/minute  1,000,000 calls/month
    {

        private string lat;
        private string lon;

        public double temperature;
        public double temperatureFeel;
        public double wind;
        public string weather;

        public OpenweatherAPIConnector(string lat, string lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
        public async Task GetWeather()
        {
            string accessKey = ConfigurationManager.AppSettings["AccessKey"];
            string requestUri = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={accessKey}&units=metric&lang=pl";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic weatherData = JsonConvert.DeserializeObject(content);
                        temperature = weatherData.main.temp;
                        temperatureFeel = weatherData.main.feels_like;
                        wind = weatherData.wind.speed;
                        weather = weatherData.weather[0].description;
                    }
                    else
                    {
                        Console.WriteLine("Nie udało się uzyskać odpowiedzi: " + response.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wystąpił wyjątek: " + e.Message);
                }
            }
        }






    }
}
