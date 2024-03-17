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
        private Mock<IHtmlTreeSearch> _mockHtmlTreeSearch;
        private CrawlerService _crawlerService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Create a fake HttpMessageHandler
            var fakeResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<html>Test HTML</html>")
            };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(fakeResponseMessage);

            // Use the handlerMock to create an HttpClient and setup the HttpClientFactory mock to return this client
            var httpClient = new HttpClient(handlerMock.Object);
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Setup other dependencies
            _mockHtmlParser = new Mock<IHtmlParser>();
            _mockLogger = new Mock<ILogger<CrawlerService>>();
            _mockHtmlTreeSearch = new Mock<IHtmlTreeSearch>();

            // Instantiate CrawlerService with the mocks
            _crawlerService = new CrawlerService(
                _mockHttpClientFactory.Object,
                _mockHtmlParser.Object,
                _mockLogger.Object,
                _mockHtmlTreeSearch.Object);
        }

        [TestMethod]
        public async Task GetHtmlContentForKeywordsAsync_ThrowsArgumentException_WhenKeywordsAreEmpty()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _crawlerService.GetHtmlContentForKeywordsAsync(new List<string>()));
        }

        [TestMethod]
        public async Task GetHtmlContentForKeywordsAsync_ReturnsContent_WhenKeywordsAreValid()
        {
            // Arrange
            var keywords = new List<string> { "test" };
            var expectedHtml = "<html>Test HTML</html>";
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(new FakeHttpMessageHandler(expectedHtml)));

            // Act
            var result = await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);

            // Assert
            Assert.AreEqual(expectedHtml, result);
        }

        [TestMethod]
        public void ReturnIndexOfGoogleSearchResults_ReturnsCorrectIndexes()
        {
            // Arrange
            var lookupURL = "http://example.com";
            var htmlFromGoogle = "<html><div><a href='http://example.com'>Example</a></div></html>";
            var expectedIndexes = new List<string> { "1" };

            _mockHtmlParser.Setup(p => p.ParseHtmlStringIntoTree(It.IsAny<string>()))
                .Returns(new HtmlNode()); // Adjust this based on your HtmlNode implementation

            _mockHtmlTreeSearch.Setup(s => s.FindDivsWithDataAsyncContext(It.IsAny<IHtmlNode>(), It.IsAny<string>()))
                .Returns(new List<IHtmlNode> { new HtmlNode { RunningIndex = 1 } }); // Adjust as needed

            // Act
            var result = _crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, htmlFromGoogle).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedIndexes, result);
        }
    }
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly string _responseContent;

        public FakeHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(_responseContent)
            });
        }
    }

}
