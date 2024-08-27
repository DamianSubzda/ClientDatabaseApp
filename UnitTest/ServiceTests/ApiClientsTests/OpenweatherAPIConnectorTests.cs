using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;
using ClientDatabaseApp.Services.APIClients;
using System.Net.Http;
using Newtonsoft.Json;

public class OpenweatherAPIConnectorTests
{
    [Fact]
    public async Task GetWeatherAsync_SuccessfulResponse_ShouldSetWeatherProperties()
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
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    main = new
                    {
                        temp = 25.3,
                        feels_like = 24.0
                    },
                    wind = new
                    {
                        speed = 3.5
                    },
                    weather = new[]
                    {
                        new { description = "clear sky" }
                    }
                }))
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var openweatherAPIConnector = new OpenweatherAPIConnector(httpClient);

        // Act
        await openweatherAPIConnector.GetWeatherAsync("52.2297", "21.0122");

        // Assert
        Assert.Equal(25.3, openweatherAPIConnector.Temperature);
        Assert.Equal(24.0, openweatherAPIConnector.TemperatureFeel);
        Assert.Equal(3.5, openweatherAPIConnector.Wind);
        Assert.Equal("clear sky", openweatherAPIConnector.Weather);
    }

    [Fact]
    public async Task GetWeatherAsync_UnsuccessfulResponse_ShouldNotSetWeatherProperties()
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
        var openweatherAPIConnector = new OpenweatherAPIConnector(httpClient);

        // Act
        await openweatherAPIConnector.GetWeatherAsync("52.2297", "21.0122");

        // Assert
        Assert.Equal(0, openweatherAPIConnector.Temperature);
        Assert.Equal(0, openweatherAPIConnector.TemperatureFeel);
        Assert.Equal(0, openweatherAPIConnector.Wind);
        Assert.Null(openweatherAPIConnector.Weather);
    }

    [Fact]
    public async Task GetWeatherAsync_ThrowsException_ShouldThrowHttpRequestException()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var openweatherAPIConnector = new OpenweatherAPIConnector(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => openweatherAPIConnector.GetWeatherAsync("52.2297", "21.0122"));
    }
}
