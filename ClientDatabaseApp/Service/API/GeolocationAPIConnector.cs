using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientDatabaseApp.Service.API
{
    public class GeolocationAPIConnector
    {
        public string IPAddress { get; private set; }
        public string[] Location { get; private set; }

        public GeolocationAPIConnector(string iPAddress)
        {
            IPAddress = iPAddress;
        }

        public async Task GetCoorAsync()
        {
            string requestUri = $"http://ip-api.com/line/{IPAddress}?fields=status,message,country,regionName,city,zip,lat,lon,query";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var responseString = await client.GetStringAsync(requestUri);
                    Location = responseString.Split('\n');
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Wystąpił wyjątek: {e.Message}");
                }
            }
        }




    }

}
