using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WebCrawler.Services;
using WebCrawler.Tree;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.Services.CrawlerServiceFunctions
{
    [TestClass]
    public class GoogleSearchResultIndexerTests
    {
        private IServiceProvider _serviceProvider;
        private CrawlerService _crawlerService;
        private IFileOperations _fileOperations;
        private string _testDataDirectory;

        [TestInitialize]
        public void TestInit()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IHtmlParser, HtmlParser>(); // Assuming this is your custom class
            services.AddSingleton<ILogger<CrawlerService>, Logger<CrawlerService>>(); // Adjust based on your logging needs
            services.AddScoped<CrawlerService>();
            services.AddScoped<HtmlTreeSearch>();
            services.AddScoped<IFileOperations, FileOperations>();

            _serviceProvider = services.BuildServiceProvider();
            _crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            _fileOperations = _serviceProvider.GetRequiredService<IFileOperations>();

            // Setup the path dynamically based on the current execution directory
            _testDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Services", "CrawlerServiceFunctions", "html");
        }
        private async Task<string> LoadHtmlFromFile(string fileName)
        {
            var path = Path.Combine(_testDataDirectory, fileName);
            return await _fileOperations.LoadFromFile(path);
        }

        [TestMethod]
        public async Task ReturnIndexOfGoogleSearchResults_LoadsFromHtmlFileAndFindsCorrectIndexes()
        {
            Assert.IsNotNull(_serviceProvider, "Service provider must be initialized.");

            // Arrange
            var lookupURL = "www.infotrack.com";
            string googleHtml = await LoadHtmlFromFile("latestHtmlFile.html");

            // Act
            var result = _crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml);

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
            Assert.AreEqual("1", result[0], "First result index should be 1.");
            Assert.AreEqual("2", result[1], "Second result index should be 2.");
        }
        [TestMethod]
        public async Task ReturnIndexes_WhenSearchingGoogleResultsFromFile_ExpectSpecificOrder()
        {
            // Ensure the service provider is properly initialized.
            Assert.IsNotNull(_serviceProvider, "Service provider is not initialized.");
            Assert.IsNotNull(_crawlerService, "Crawler Service is not initialized.");

            // Arrange
            var lookupURL = "www.infotrack.com";

            // Using a relative path to load the test HTML file.
            var fileName = "latestHtmlFile2.html";
            var testDataPath = Path.Combine(_testDataDirectory, fileName); // Assuming _testDataDirectory is set correctly in TestInit.
            string googleHtml = await _fileOperations.LoadFromFile(testDataPath);
            Assert.AreNotEqual(string.Empty, googleHtml, "The file loaded should not be an empty string.");

            // Act
            var result = _crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml); // Assuming this method is asynchronous.

            // Assert
            // Verify that the results match the expected indexes. Provide meaningful assertion messages.
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
            Assert.AreEqual("1", result[0], "The first index should be 1.");
            Assert.AreEqual("3", result[1], "The second index should be 3.");
            Assert.AreEqual("8", result[2], "The third index should be 8.");
        }
        [TestMethod]
        public async Task ReturnIndexes_FromWebSearch_ShouldFindKeywords()
        {
            // Ensure the service provider is properly initialized.
            Assert.IsNotNull(_serviceProvider, "Service provider is not initialized.");
            Assert.IsNotNull(_crawlerService, "Crawler Service is not initialized.");

            // Arrange
            var keywords = new List<string> { "efiling", "integration" };
            var lookupURL = "www.infotrack.com";

            // Act
            string googleHtml = await _crawlerService.CreateHTMLFileFromWeb(keywords);
            var result = _crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml); // Assuming it's an async method.

            // Assert
            // Verify the result is not null and contains at least one entry, indicating a successful search.
            Assert.IsNotNull(result, "Expected non-null result from web search.");
            Assert.IsTrue(result.Count > 0, "Expected at least one search result index.");
        }

        [TestMethod]
        public async Task Should_ReturnNonEmptySearchResults_ForInfotrackKeyword_FromWeb()
        {
            // Ensure that the service provider has been properly initialized before proceeding with the test.
            Assert.IsNotNull(_serviceProvider, "Service provider is not initialized, which is required for the test setup.");
            // Ensure the service provider is properly initialized.
            Assert.IsNotNull(_serviceProvider, "Service provider is not initialized.");
            Assert.IsNotNull(_crawlerService, "Crawler Service is not initialized.");

            var keywords = new List<string> { "infotrack" }; // Directly initialize the keywords list in the same line for clarity.
            var lookupURL = "www.infotrack.com";

            // Act: Fetch the HTML content based on the specified keywords and then search for the lookupURL within that content.
            string googleHtml = await _crawlerService.CreateHTMLFileFromWeb(keywords); // Assuming CreateHTMLFileFromWeb is an async method and should be awaited.
            Assert.AreNotEqual(string.Empty, googleHtml, "The file loaded should not be an empty string.");

            var result = _crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml); // Assuming this method might be async. If not, remove the await keyword.

            // Assert: Directly validate the outcomes of the action to ensure that the search results are as expected.
            Assert.IsNotNull(result, "The search results should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one search result index indicating a successful find.");
        }
    }
}
