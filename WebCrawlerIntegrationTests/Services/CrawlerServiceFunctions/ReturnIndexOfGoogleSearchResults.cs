﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using WebCrawler.Services;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.Services.CrawlerServiceFunctions
{
    [TestClass]
    public class GoogleSearchResultIndexerTests
    {
        private readonly IHttpClientFactory? _httpClientFactory;
        private readonly IHtmlParser? _htmlParser;

        public GoogleSearchResultIndexerTests()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            _htmlParser = new HtmlParser();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {

        }

        [TestMethod]
        public async Task ReturnIndexOfGoogleSearchResults_UsesRealHttpClient()
        {
            // Assuming YourClass is constructed with IHttpClientFactory and it's designed to hit a testable endpoint
            var yourClassInstance = new CrawlerService(_httpClientFactory, _htmlParser);

            // Replace "https://example.com" with the URL of your live or local testing environment
            // Ensure this environment is predictable and available for integration testing
            var url = StringUtilities.CreateLookupURL(100, ["efiling", "integration"]);
            var result = await yourClassInstance.ReturnIndexOfGoogleSearchResults(new List<string> { "efiling", "integration" }, url, "100");

            // Assert based on expected behavior of the live service
            // This will vary depending on what the service is expected to return
            // For example, if you know the service should return at least one match, you could assert the list is not empty
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}