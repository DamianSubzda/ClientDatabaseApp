using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientDatabaseApp.Service.API
{
    public class WeatherstackAPIConnector
    {
        public BitmapImage photo;
        public event Action<BitmapImage> OnImageLoaded;
        public async Task GetImage()
        {
            // Twój klucz dostępu do API Unsplash
            string accessKey = "hQ7ed2zFoVzuMBp7QkZGM4kBL2Xrk-1CdoD31ac6l-g";

            // Endpoint API Unsplash do losowego zdjęcia
            string requestUri = $"https://api.unsplash.com/photos/random?client_id={accessKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic photo = JsonConvert.DeserializeObject(content);

                        string photoUrl = photo.urls.full;

                        Console.WriteLine($"Zdjęcie URL: {photoUrl}");
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
