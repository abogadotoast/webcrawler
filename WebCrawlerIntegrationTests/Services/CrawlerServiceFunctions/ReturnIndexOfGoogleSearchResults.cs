using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WebCrawler.Services;
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

            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task ReturnIndexOfGoogleSearchResults_IntegrationTest()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var logger = _serviceProvider.GetRequiredService<ILogger<CrawlerService>>();
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var htmlParser = _serviceProvider.GetRequiredService<IHtmlParser>();
            var crawlerService = new CrawlerService(httpClientFactory, htmlParser, logger);

            var keywords = new List<string> { "efiling", "integration" };
            var lookupURL = "https://www.infotrack.com";
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, keywords);

            // Asserts to verify behavior without relying on fixed positions due to potential changes in Google's search results
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
