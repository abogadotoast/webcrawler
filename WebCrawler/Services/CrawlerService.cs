using System;
using System.Text.RegularExpressions;
using WebCrawler.Services;
using WebCrawler.Tree;
using WebCrawler.Utilities;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebCrawler.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class CrawlerService
    {
        private readonly HttpClient _httpClient;
        private readonly IHtmlParser _htmlParser;
        private readonly ILogger<CrawlerService> _logger;
        private readonly HtmlTreeSearch _htmlTreeSearch;


        public CrawlerService(IHttpClientFactory httpClientFactory, IHtmlParser htmlParser, ILogger<CrawlerService> logger, HtmlTreeSearch htmlTreeSearch)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");
            _htmlParser = htmlParser;
            _logger = logger;
            _htmlTreeSearch = htmlTreeSearch;
        }

        public async Task<IList<string>> ReturnIndexOfGoogleSearchResults(string lookupURL, IList<string> keywords)
        {
            const int ONE_HUNDRED_RESULTS_FROM_GOOGLE = 100;
            List<string> matchingIndexes = new List<string>();

            try
            {
                string appendedLookupURL = StringUtilities.AppendUrlAndQToWebsiteBeingSearched(lookupURL);
                string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ONE_HUNDRED_RESULTS_FROM_GOOGLE, keywords);
                var html = await _httpClient.GetStringAsync(googleUrlWithKeywords);
                var rootNode = _htmlParser.ParseHtmlStringIntoTree(html);
                var matchingNodes = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, appendedLookupURL);

                matchingIndexes.AddRange(matchingNodes.Select(node => node.RunningIndex.ToString()));
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "An HTTP request error occurred while trying to fetch search results.");
                // Optionally, rethrow to let the caller handle it, or handle it here
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to process search results.");
                // Depending on the application's needs, you might want to rethrow, return a default value, or handle the error in some other way.
            }

            return matchingIndexes;
        }
    }
}
