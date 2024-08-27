using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;
using ClientDatabaseApp.Services.APIClients;
using System.Net.Http;
using Newtonsoft.Json;

public class IpfyAPIConnectorTests
{
    [Fact]
    public async Task GetIpAsync_SuccessfulResponse_ShouldSetIPAddress()
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
                    ip = "123.123.123.123"
                }))
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var ipifyAPIConnector = new IpifyAPIConnector(httpClient);

        // Act
        await ipifyAPIConnector.GetIpAsync();

        // Assert
        Assert.Equal("123.123.123.123", ipifyAPIConnector.IPAddress);
    }

    [Fact]
    public async Task GetIpAsync_UnsuccessfulResponse_ShouldNotSetIPAddress()
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
        var ipifyAPIConnector = new IpifyAPIConnector(httpClient);

        // Act
        await ipifyAPIConnector.GetIpAsync();

        // Assert
        Assert.Null(ipifyAPIConnector.IPAddress);
    }

    [Fact]
    public async Task GetIpAsync_ThrowsException_ShouldThrowHttpRequestException()
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
        var ipifyAPIConnector = new IpifyAPIConnector(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => ipifyAPIConnector.GetIpAsync());
    }
}
