using Microsoft.Extensions.Configuration;
using Moq;
using ShortenerAPI.Entities;
using ShortenerAPI.Services;
using ShortenerAPI.Services.Interfaces;
using System;
using Xunit;

namespace ShortenerAPI_tests.Services
{
    public class ShortenerServiceTest
    {
        private readonly UrlModel _testUrlModel;
        private readonly Mock<IDatabaseService> _databaseServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ShortenenerService _shortenenerService;
        public ShortenerServiceTest()
        {
            _databaseServiceMock = new Mock<IDatabaseService>();
            _configurationMock = new Mock<IConfiguration>();
            _shortenenerService = new ShortenenerService(_databaseServiceMock.Object, _configurationMock.Object);
            _testUrlModel = new UrlModel
            {
                CreatedAt = DateTime.Now,
                HostName = "test_host_name",
                LongUrl = "test_long_url",
                ShortUrl = "test_short_url",
                Id = 0
            };
        }

        [Fact]
        public void GetLongUrlForShortUrl_UrlExist()
        {
            _databaseServiceMock.Setup(s => s.GetLongUrlForShortUrl("test_url")).Returns(_testUrlModel);
            var actual = _shortenenerService.GetLongUrlForShortUrl("test_url");
            Assert.Equal(actual, _testUrlModel.LongUrl);
        }

        [Fact]
        public void GetLongUrlForShortUrl_UrlNotFound()
        {
            _databaseServiceMock.Setup(s => s.GetLongUrlForShortUrl("test_url"));
            _configurationMock.SetupGet(s => s[It.IsAny<string>()]).Returns("default_url");
            var defaultUrlIfExist = _shortenenerService.GetLongUrlForShortUrl("test_url");
            Assert.Equal("default_url", defaultUrlIfExist);

            _configurationMock.SetupGet(s => s[It.IsAny<string>()]);
            var defaultUrlIfNotFound = _shortenenerService.GetLongUrlForShortUrl("test_url");
            Assert.Equal(string.Empty, defaultUrlIfNotFound);
        }

        [Fact]
        public void ShortenUrl_UrlExist()
        {
            _databaseServiceMock.Setup(s => s.GetShortUrlForLongUrl("http://test.pl/")).Returns(_testUrlModel);
            var actual = _shortenenerService.ShortenUrl("test.pl");
            Assert.Equal(_testUrlModel.ToShort(), actual);
        }

        [Fact]
        public void ShortenUrl_GenerateNew()
        {
            var actualIfSaveFailed = _shortenenerService.ShortenUrl("test.pl");
            Assert.Equal(actualIfSaveFailed, string.Empty);

            _databaseServiceMock.Setup(s => s.SaveShortenResult(It.IsAny<string>(), It.IsAny<string>())).Returns(_testUrlModel);
            var actualIfSaveSuccess = _shortenenerService.ShortenUrl("test.pl");
            Assert.Equal(_testUrlModel.ToShort(), actualIfSaveSuccess);
        }
    }
}