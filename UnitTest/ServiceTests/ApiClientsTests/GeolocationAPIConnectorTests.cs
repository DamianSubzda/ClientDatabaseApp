using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;
using ClientDatabaseApp.Services.APIClients;
using System.Net.Http;
using Newtonsoft.Json;
namespace UnitTest.ServiceTests.ApiClientsTests
{
    public class GeolocationAPIConnectorTests
    {
        [Fact]
        public async Task GetCoorAsync_SuccessfulResponse_ShouldSetLatLonCity()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        lat = "52.2297",
                        lon = "21.0122",
                        city = "Warsaw"
                    }))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var geolocationAPIConnector = new GeolocationAPIConnector(httpClient);

            // Act
            await geolocationAPIConnector.GetCoorAsync("127.0.0.1");

            // Assert
            Assert.Equal("52.2297", geolocationAPIConnector.Lat);
            Assert.Equal("21.0122", geolocationAPIConnector.Lon);
            Assert.Equal("Warsaw", geolocationAPIConnector.City);
        }

        [Fact]
        public async Task GetCoorAsync_UnsuccessfulResponse_ShouldNotSetLatLonCity()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var geolocationAPIConnector = new GeolocationAPIConnector(httpClient);

            // Act
            await geolocationAPIConnector.GetCoorAsync("127.0.0.1");

            // Assert
            Assert.Null(geolocationAPIConnector.Lat);
            Assert.Null(geolocationAPIConnector.Lon);
            Assert.Null(geolocationAPIConnector.City);
        }

        [Fact]
        public async Task GetCoorAsync_ThrowsException_ShouldThrowHttpRequestException()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Request failed"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var geolocationAPIConnector = new GeolocationAPIConnector(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => geolocationAPIConnector.GetCoorAsync("127.0.0.1"));
        }
    }
}