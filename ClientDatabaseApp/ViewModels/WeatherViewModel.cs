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

        private readonly IIpifyAPIConnector _ipifyAPIConnector;
        private readonly IGeolocationAPIConnector _geolocationAPIConnector;
        private readonly IOpenweatherAPIConnector _openweatherAPIConnector;

        public WeatherViewModel(IIpifyAPIConnector ipifyAPIConnector, IGeolocationAPIConnector geolocationAPIConnector, IOpenweatherAPIConnector openweatherAPIConnector)
        {
            _ipifyAPIConnector = ipifyAPIConnector;
            _geolocationAPIConnector = geolocationAPIConnector;
            _openweatherAPIConnector = openweatherAPIConnector;

            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            await _ipifyAPIConnector.GetIpAsync();
            await _geolocationAPIConnector.GetCoorAsync(_ipifyAPIConnector.IPAddress);
            await _openweatherAPIConnector.GetWeatherAsync(_geolocationAPIConnector.Lat, _geolocationAPIConnector.Lon);

            Temperature = Math.Round(_openweatherAPIConnector.Temperature, 1).ToString() + " C.";
            TemperatureFeel = Math.Round(_openweatherAPIConnector.TemperatureFeel, 1).ToString() + " C.";
            Wind = Math.Round(_openweatherAPIConnector.Wind * 36 / 10, 1).ToString() + " km/h.";
            Weather = _openweatherAPIConnector.Weather + '.';
            City = _geolocationAPIConnector.City + '.';
        }


    }
}
