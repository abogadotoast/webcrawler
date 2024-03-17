using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCrawler.Controllers;
using WebCrawler.Services;

namespace WebCrawlerUnitTests.Controllers
{
    [TestClass]
    public class WebCrawlerControllerTests
    {
        private Mock<ICrawlerService> _mockCrawlerService;
        private WebCrawlerController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockCrawlerService = new Mock<ICrawlerService>();
            _controller = new WebCrawlerController(_mockCrawlerService.Object);
        }

        [TestMethod]
        public async Task GetAsync_UrlFound_ReturnsIndices()
        {
            // Arrange
            var keywords = new List<string> { "efiling", "integration" };
            var urlToFind = "www.infotrack.com";
            _mockCrawlerService.Setup(service => service.GetHtmlContentForKeywordsAsync(keywords))
                               .ReturnsAsync("mockHtmlContent");
            _mockCrawlerService.Setup(service => service.ReturnIndexOfGoogleSearchResults(urlToFind, "mockHtmlContent"))
                               .Returns(new List<string> { "1", "2" });

            // Act
            var result = await _controller.GetAsync(keywords, urlToFind);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("1, 2", okResult.Value);
        }

        [TestMethod]
        public async Task GetAsync_UrlNotFound_ReturnsZero()
        {
            // Arrange
            var keywords = new List<string> { "nonexistent" };
            var urlToFind = "www.unknown.com";
            _mockCrawlerService.Setup(service => service.GetHtmlContentForKeywordsAsync(keywords))
                               .ReturnsAsync("mockHtmlContent");
            _mockCrawlerService.Setup(service => service.ReturnIndexOfGoogleSearchResults(urlToFind, "mockHtmlContent"))
                               .Returns(new List<string>());

            // Act
            var result = await _controller.GetAsync(keywords, urlToFind);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("0", okResult.Value);
        }

        [TestMethod]
        public async Task GetAsync_MissingKeywordsOrUrl_ReturnsBadRequest()
        {
            // Arrange
            var keywords = new List<string>();
            var urlToFind = ""; // Empty or null to simulate missing data

            // Act
            var result = await _controller.GetAsync(keywords, urlToFind);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Keywords and URL to find must be provided.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetAsync_ServiceThrowsException_ReturnsServerError()
        {
            // Arrange
            var keywords = new List<string> { "efiling" };
            var urlToFind = "www.infotrack.com";
            _mockCrawlerService.Setup(service => service.GetHtmlContentForKeywordsAsync(keywords))
                               .ThrowsAsync(new System.Exception("Internal server error"));

            // Act
            var result = await _controller.GetAsync(keywords, urlToFind);

            // Assert
            var serverErrorResult = result.Result as ObjectResult;
            Assert.IsNotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
            Assert.AreEqual("An error occurred while processing your request. Please try again later.", serverErrorResult.Value);
        }
    }
    }
