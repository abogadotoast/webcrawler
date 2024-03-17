using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Services;
using WebCrawler.DataStructures;
using WebCrawler.Utilities;

namespace WebCrawlerUnitTests.ServicesTests
{
    [TestClass]
    public class CrawlerServiceTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<IHtmlParser> _mockHtmlParser;
        private Mock<ILogger<CrawlerService>> _mockLogger;
        private Mock<HtmlTreeSearch> _mockHtmlTreeSearch;
        private HttpClient _mockHttpClient;
        private CrawlerService _crawlerService;

        [TestInitialize]
        public void Initialize()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            _mockHttpClient = new HttpClient(handlerMock.Object);
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(_mockHttpClient);

            _mockHtmlParser = new Mock<IHtmlParser>();
            _mockLogger = new Mock<ILogger<CrawlerService>>();
            _mockHtmlTreeSearch = new Mock<HtmlTreeSearch>();

            // Setup the HttpMessageHandler mock
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("<html></html>"),
                });

            _crawlerService = new CrawlerService(_mockHttpClientFactory.Object, _mockHtmlParser.Object, _mockLogger.Object, _mockHtmlTreeSearch.Object);
        }
        [TestMethod]
        public async Task GetHtmlContentForKeywordsAsync_ReturnsContent_WhenKeywordsAreValid()
        {
            var keywords = new List<string> { "example" };
            var result = await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("<html></html>"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetHtmlContentForKeywordsAsync_ThrowsArgumentException_WhenKeywordsAreEmpty()
        {
            var keywords = new List<string>();
            await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);
        }

    }

}
