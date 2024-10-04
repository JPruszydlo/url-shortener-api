using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using ShortenerAPI.Models;
using ShortenerAPI.Services;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ShortenerAPI_tests.Services
{
    public class RecaptchaServiceTest
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly RecaptchaService _recaptchaService;
        private readonly ReCaptchaResponse _failedRecaptchaResponse;
        private readonly ReCaptchaResponse _successRecaptchaResponse;
        public RecaptchaServiceTest()
        {
            _configurationMock = new Mock<IConfiguration>();

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var client = new HttpClient(_httpMessageHandlerMock.Object);
            _recaptchaService = new RecaptchaService(_configurationMock.Object, client);
            _failedRecaptchaResponse = new ReCaptchaResponse();
            _successRecaptchaResponse = new ReCaptchaResponse()
            {
                Score = 0.8f,
                HostName = "test_host_name",
                Success = true,
                Action = "shortenurl"
            };
        }

        [Fact]
        public void VerifyRecaptcha_Success()
        {
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage() { 
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(_successRecaptchaResponse)),
                });
            _configurationMock.SetupGet(s => s[It.IsAny<string>()]).Returns(_successRecaptchaResponse.HostName);
            var resultSuccessfull = _recaptchaService.VerifyRecaptcha("token");
            Assert.True(resultSuccessfull.Result);
        }
        [Fact]
        public void VerifyRecaptcha_Failed()
        {
            _configurationMock.SetupGet(s => s["reCaptcha:secretCode"]).Returns(string.Empty);
            var resultIfTokenEmpty = _recaptchaService.VerifyRecaptcha("");
            Assert.False(resultIfTokenEmpty.Result);

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized});
            var resultIfNotAuthorized = _recaptchaService.VerifyRecaptcha("token");
            Assert.False(resultIfNotAuthorized.Result);

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(_failedRecaptchaResponse)),
                });
            var resultIfRecaptchaVerificationFailed = _recaptchaService.VerifyRecaptcha("token");
            Assert.False(resultIfRecaptchaVerificationFailed.Result);
        }
    }
}
