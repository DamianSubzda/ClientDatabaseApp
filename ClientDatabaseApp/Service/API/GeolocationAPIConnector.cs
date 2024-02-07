using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientDatabaseApp.Service.API //Requests -> nolimit
{
    public class GeolocationAPIConnector
    {
        public string IPAddress { get; private set; }
        public string Lat { get; private set; }
        public string Lon { get; private set; }
        public string City { get; private set; }

        public GeolocationAPIConnector(string iPAddress)
        {
            IPAddress = iPAddress;
        }

        public async Task GetCoorAsync()
        {
            string requestUri = $"http://ip-api.com/json/{IPAddress}?fields=status,message,country,regionName,city,zip,lat,lon,query&lang=pl";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic locationData = JsonConvert.DeserializeObject(content);
                        Lat = locationData.lat;
                        Lon = locationData.lon;
                        City = locationData.city;
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
