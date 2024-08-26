using ClientDatabaseApp.Services.APIClients;
using System;

namespace ClientDatabaseApp.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        private string _temperature;
        private string _temperatureFeel;
        private string _wind;
        private string _weather;
        private string _city;

        public string Temperature
        {
            get => _temperature;
            set => SetField(ref _temperature, value, nameof(Temperature));
        }
        public string TemperatureFeel
        {
            get => _temperatureFeel;
            set => SetField(ref _temperatureFeel, value, nameof(TemperatureFeel));
        }
        public string Wind
        {
            get => _wind;
            set => SetField(ref _wind, value, nameof(Wind));
        }
        public string Weather
        {
            get => _weather;
            set => SetField(ref _weather, value, nameof(Weather));
        }
        public string City
        {
            get => _city;
            set => SetField(ref _city, value, nameof(City));
        }

        public WeatherViewModel()
        {
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            IpifyAPIConnector ipify = new IpifyAPIConnector();
            await ipify.GetIp();
            GeolocationAPIConnector geolocation = new GeolocationAPIConnector(ipify.IPAddress);
            await geolocation.GetCoorAsync();
            OpenweatherAPIConnector openweather = new OpenweatherAPIConnector(geolocation.Lat, geolocation.Lon);
            await openweather.GetWeather();
            Temperature = Math.Round(openweather.temperature, 1).ToString() + " C.";
            TemperatureFeel = Math.Round(openweather.temperatureFeel, 1).ToString() + " C.";
            Wind = Math.Round(openweather.wind * 36 / 10, 1).ToString() + " km/h.";
            Weather = openweather.weather + '.';
            City = geolocation.City + '.';
        }


    }
}
