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
        [TestMethod]
        public async Task GetAsync_ReturnsExpectedUrls_WhenKeywordsAndUrlProvided()
        {
            // Arrange
            var mockCrawlerService = new Mock<ICrawlerService>();
            IList<string> keywords = ["example"];
            string urlToFindOnList = "http://example.com";
            IList<string> expectedUrls = [urlToFindOnList];

            mockCrawlerService.Setup(s => s.GetHtmlContentForKeywordsAsync(It.IsAny<IList<string>>()))
                .ReturnsAsync("dummyHtmlContent");

            mockCrawlerService.Setup(s => s.ReturnIndexOfGoogleSearchResults(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(expectedUrls);

            var controller = new WebCrawlerController(mockCrawlerService.Object);

            // Act
            var actionResult = await controller.GetAsync(keywords, urlToFindOnList);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IList<string>));
            var resultUrls = okResult.Value as IList<string>;
            Assert.IsNotNull(resultUrls);
            Assert.AreEqual(expectedUrls.Count, resultUrls.Count);
            Assert.AreEqual(expectedUrls[0], resultUrls[0]);

            // Verify that the crawler service methods were called as expected
            mockCrawlerService.Verify(s => s.GetHtmlContentForKeywordsAsync(It.IsAny<IList<string>>()), Times.Once);
            mockCrawlerService.Verify(s => s.ReturnIndexOfGoogleSearchResults(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
