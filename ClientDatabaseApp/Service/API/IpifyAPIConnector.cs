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
    public class IpifyAPIConnector
    {
        public string IPAddress { get; private set; }

        public IpifyAPIConnector()
        {
        }

        public async Task GetIp()
        {
            string requestUri = "https://api.ipify.org";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var ipAddress = await client.GetStringAsync(requestUri);
                    IPAddress = ipAddress;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Wystąpił wyjątek: {e.Message}");
                }
            }
        }




    }
}
