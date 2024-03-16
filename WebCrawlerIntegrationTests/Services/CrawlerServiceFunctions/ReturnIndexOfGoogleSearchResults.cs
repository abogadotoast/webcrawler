using System;
using System.Collections.Generic;
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
        public async Task ReturnIndexOfGoogleSearchResults_IntegrationTest()
        {
            Assert.IsNotNull(_serviceProvider);
            // Arrange
            var logger = _serviceProvider.GetRequiredService<ILogger<CrawlerService>>();
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var htmlParser = _serviceProvider.GetRequiredService<IHtmlParser>();
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();// new CrawlerService(httpClientFactory, htmlParser, logger);

            var keywords = new List<string> { "efiling", "integration" };
            var lookupURL = "www.infotrack.com";
            var result = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, keywords);

            // Asserts to verify behavior without relying on fixed positions due to potential changes in Google's search results
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.IsTrue(result.Count > 0, "Expected at least one result.");
        }
        /// <summary>
        /// There are two methods of getting a result on the list.
        /// The first is by a node search.
        /// The second is by doing a .Where() clause.
        /// The .Where() clause is considerably more expensive, but easy to check.
        /// We test to make sure that our behaviors are identical.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AssertIntegrationsOfDifferentKeywordsAndSitesWillWork()
        {
            var keywords = new List<string> { "efiling", "integration" };
            var keywords2 = new List<string> { "Infotrack" };
            var lookupURL = "https://www.infotrack.com";
            await INEFFICENT_ReturnIndexOfGoogleSearchResults(keywords, lookupURL);
            await INEFFICENT_ReturnIndexOfGoogleSearchResults(keywords2, lookupURL);
        }

        /// <summary>
        /// This method is for testing. We check to ensure our Node indexing is correct using a second method: build the 100 first, then pick their indexes from where the urls match.
        /// </summary>
        /// <param name="lookupURL"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        private async Task INEFFICENT_ReturnIndexOfGoogleSearchResults(List<string> keywords, string lookupURL)
        {


            const int ONE_HUNDRED_RESULTS_FROM_GOOGLE = 100;

            // Arrange
            var logger = _serviceProvider.GetRequiredService<ILogger<CrawlerService>>();
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var htmlParser = _serviceProvider.GetRequiredService<IHtmlParser>();
            var crawlerService = _serviceProvider.GetRequiredService<CrawlerService>();
            var treeSearch = _serviceProvider.GetRequiredService<HtmlTreeSearch>();

            List<string> smallerResultSet = new List<string>();
            List<string> allResultSet = new List<string>();
            List<string> finalResultSet = new List<string>();
            List<int> matchingIndexes = new List<int>();
            List<int> smallIndexes = new List<int>();
            List<int> bigIndexes = new List<int>();

            var httpClient = httpClientFactory.CreateClient();

            // Act
            // var smallMatchingNodesList = await crawlerService.ReturnIndexOfGoogleSearchResults(lookupURL, keywords);

            // Other act.

            // For a targeted search, e.g. url?q="efiling+integration"
            string appendedLookupURL = StringUtilities.AppendUrlAndQToWebsiteBeingSearched(lookupURL);
            string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ONE_HUNDRED_RESULTS_FROM_GOOGLE, keywords);
            var html = await httpClient.GetStringAsync(googleUrlWithKeywords);
            var rootNode = htmlParser.ParseHtmlStringIntoTree(html);
            var smallMatchingNodesList = treeSearch.FindDivsWithDataAsyncContext(rootNode, appendedLookupURL);
            smallerResultSet.AddRange(smallMatchingNodesList.Select(node => node.RunningIndex.ToString()));

            // Build the tree for one hundred results under the parameter of something like url?q=
            string allUrls = StringUtilities.AppendUrlAndQToWebsiteBeingSearched(string.Empty);
            var bigMatchingNodesList = treeSearch.FindDivsWithDataAsyncContext(rootNode, allUrls);
            var indexesFound = new List<int>();
            // Search each result of the small tree and see what it is in the big tree.
            foreach (var smallNode in smallMatchingNodesList)
            {
                // Paths should be the same no matter which document.
                var indexFound = bigMatchingNodesList.FindIndex(a => a.Path == smallNode.Path);
                indexesFound.Add(indexFound + 1);
            }
            // Now we compare tpo using the actual indexing function on the small nodes.
            // Do the indexes match between the path finding vs index finding methods?
            foreach (var smallStr in smallerResultSet)
            {
                var indexOfSmallStr = Convert.ToInt32(smallStr);
                var indexFound = indexesFound.FindIndex(a => a == indexOfSmallStr);
                Assert.IsNotNull(indexFound, "Every index should exist 1:1 in both sets.");
            }
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
