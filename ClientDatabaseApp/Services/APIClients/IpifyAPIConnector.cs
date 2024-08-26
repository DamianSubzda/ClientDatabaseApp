using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Services.APIClients //Requests -> nolimit
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
