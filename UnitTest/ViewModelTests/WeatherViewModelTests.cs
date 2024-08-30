using Moq;
using Xunit;
using ClientDatabaseApp.Services.APIClients;
using ClientDatabaseApp.ViewModels;
using System.Threading.Tasks;

namespace UnitTest.ViewModelsTests
{
    public class WeatherViewModelTests
    {
        private readonly Mock<IIpifyAPIConnector> _ipifyAPIConnectorMock;
        private readonly Mock<IGeolocationAPIConnector> _geolocationAPIConnectorMock;
        private readonly Mock<IOpenweatherAPIConnector> _openweatherAPIConnectorMock;
        private readonly WeatherViewModel _viewModel;

        public WeatherViewModelTests()
        {
            _ipifyAPIConnectorMock = new Mock<IIpifyAPIConnector>();
            _geolocationAPIConnectorMock = new Mock<IGeolocationAPIConnector>();
            _openweatherAPIConnectorMock = new Mock<IOpenweatherAPIConnector>();

            _viewModel = new WeatherViewModel(
                _ipifyAPIConnectorMock.Object,
                _geolocationAPIConnectorMock.Object,
                _openweatherAPIConnectorMock.Object
            );
        }

        [Fact]
        public void InitializeAsync_ShouldSetProperties_WhenApiCallsSucceed()
        {
            // Arrange
            _ipifyAPIConnectorMock.Setup(api => api.GetIpAsync()).Returns(Task.CompletedTask);
            _ipifyAPIConnectorMock.Setup(api => api.IPAddress).Returns("127.0.0.1");

            _geolocationAPIConnectorMock.Setup(api => api.GetCoorAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _geolocationAPIConnectorMock.Setup(api => api.Lat).Returns("51.5074");
            _geolocationAPIConnectorMock.Setup(api => api.Lon).Returns("-0.1278");
            _geolocationAPIConnectorMock.Setup(api => api.City).Returns("London");

            _openweatherAPIConnectorMock.Setup(api => api.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _openweatherAPIConnectorMock.Setup(api => api.Temperature).Returns(15.5);
            _openweatherAPIConnectorMock.Setup(api => api.TemperatureFeel).Returns(14.5);
            _openweatherAPIConnectorMock.Setup(api => api.Wind).Returns(5.0);
            _openweatherAPIConnectorMock.Setup(api => api.Weather).Returns("Sunny");

            // Act
            _viewModel.InitializeAsync();

            // Asser
            Assert.Equal("15,5 C.", _viewModel.Temperature);
            Assert.Equal("14,5 C.", _viewModel.TemperatureFeel);
            Assert.Equal("18 km/h.", _viewModel.Wind);
            Assert.Equal("Sunny.", _viewModel.Weather);
            Assert.Equal("London.", _viewModel.City);
        }

        [Fact]
        public void InitializeAsync_ShouldCallApiMethodsInOrder()
        {
            // Arrange
            _ipifyAPIConnectorMock.Setup(api => api.GetIpAsync()).Returns(Task.CompletedTask);
            _ipifyAPIConnectorMock.Setup(api => api.IPAddress).Returns("127.0.0.1");

            _geolocationAPIConnectorMock.Setup(api => api.GetCoorAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _geolocationAPIConnectorMock.Setup(api => api.Lat).Returns("51.5074");
            _geolocationAPIConnectorMock.Setup(api => api.Lon).Returns("-0.1278");

            _openweatherAPIConnectorMock.Setup(api => api.GetWeatherAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            _viewModel.InitializeAsync();

            // Assert
            _ipifyAPIConnectorMock.Verify(api => api.GetIpAsync(), Times.AtLeastOnce);
            _geolocationAPIConnectorMock.Verify(api => api.GetCoorAsync("127.0.0.1"), Times.AtLeastOnce);
            _openweatherAPIConnectorMock.Verify(api => api.GetWeatherAsync("51.5074", "-0.1278"), Times.AtLeastOnce);
        }
    }
}
