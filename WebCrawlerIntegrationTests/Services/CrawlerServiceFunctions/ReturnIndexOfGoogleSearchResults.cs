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
        private static IServiceProvider _serviceProvider;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IHtmlParser, HtmlParser>(); // Assuming HtmlParser is your custom class implementing IHtmlParser
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddScoped<CrawlerService>();
            services.AddScoped<HtmlTreeSearch>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task BASE_CASE_ReturnIndexOfGoogleSearchResults_IntegrationTest_From_File()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            var keywords = new List<string> { "efiling", "integration" };
            var lookupURL = "www.infotrack.com";
            // Act
            string googleHtml = await crawlerService.CreateHTMLFileForParsing(keywords, false);
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml);
            // Asserts to verify behavior is exactly the same as the file.
            // The file is a prerecorded copy of the Google site on a local machine.
            Assert.IsTrue(result[0] == "1");
            Assert.IsTrue(result[1] == "2");
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
        }
        [TestMethod]
        public async Task ALTERNATIVE_CASE_ReturnIndexOfGoogleSearchResults_IntegrationTest_From_File()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            var keywords = new List<string> { "infotrack" };
            var lookupURL = "www.infotrack.com";
            // Act
            string googleHtml = await crawlerService.CreateHTMLFileForParsing(keywords, false);
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml);
            // Asserts to verify behavior is exactly the same as the file.
            // The file is a prerecorded copy of the Google site on a local machine.
            Assert.IsTrue(result[0] == "1");
            Assert.IsTrue(result[1] == "3");
            Assert.IsTrue(result[1] == "8");
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
        }
        [TestMethod]
        public async Task BASE_CASE_ReturnIndexOfGoogleSearchResults_IntegrationTest_From_Internet()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            var keywords = new List<string> { "efiling", "integration" };
            var lookupURL = "www.infotrack.com";
            // Act
            string googleHtml = await crawlerService.CreateHTMLFileForParsing(keywords, true);
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml);
            // The checks are a little bit less serious with this one - we just check if the response exists, since the file one already checks to see if the parsing is done correctly.
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
        }
        [TestMethod]
        public async Task ALTERNATIVE_CASE_ReturnIndexOfGoogleSearchResults_IntegrationTest_From_Internet()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            var keywords = new List<string> { "infotrack"};
            var lookupURL = "www.infotrack.com";
            // Act
            string googleHtml = await crawlerService.CreateHTMLFileForParsing(keywords, true);
            var path2 = @"C:\Users\gugra\source\repos\WebCrawlerProj\WebCrawler\html\latestHtmlFile2.html";
            await crawlerService.PrintHtmlToFile(googleHtml, path2);
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, googleHtml);
            // The checks are a little bit less serious with this one - we just check if the response exists, since the file one already checks to see if the parsing is done correctly.
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
