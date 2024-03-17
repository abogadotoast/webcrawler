using WebCrawler.DataStructures;
using WebCrawler.Utilities;

namespace WebCrawler.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class CrawlerService : ICrawlerService
    {
        private readonly HttpClient _httpClient;
        private readonly IHtmlParser _htmlParser;
        private readonly ILogger<CrawlerService> _logger;
        private readonly IHtmlTreeSearch _htmlTreeSearch;


        public CrawlerService(IHttpClientFactory httpClientFactory,
            IHtmlParser htmlParser, 
            ILogger<CrawlerService> logger, 
            IHtmlTreeSearch htmlTreeSearch)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0");
            _htmlParser = htmlParser;
            _logger = logger;
            _htmlTreeSearch = htmlTreeSearch;
        }
        public async Task<string> GetHtmlContentForKeywordsAsync(IList<string> keywords)
        {
            if (keywords == null || !keywords.Any())
            {
                throw new ArgumentException("Keywords list cannot be null or empty.", nameof(keywords));
            }

            const int ResultsCount = 100; // Constants should have meaningful names and follow PascalCasing.
            string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ResultsCount, keywords);

            try
            {
                string googleHtml = await _httpClient.GetStringAsync(googleUrlWithKeywords);
                return googleHtml;
            }
            catch (HttpRequestException ex)
            {
                // Consider logging the exception details and/or rethrowing a more specific exception
                throw new InvalidOperationException("Failed to fetch HTML content from Google.", ex);
            }
        }

        public IList<string> ReturnIndexOfGoogleSearchResults(string lookupURL, string htmlFromGoogle)
        {

            List<string> matchingIndexes = new List<string>();
            try
            {
                var rootNode = _htmlParser.ParseHtmlStringIntoTree(htmlFromGoogle);
                var matches = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, lookupURL);
                matchingIndexes.AddRange(matches.Select(node => node.RunningIndex.ToString()));
                return matchingIndexes;
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
