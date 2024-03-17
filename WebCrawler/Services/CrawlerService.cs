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

    /// <summary>
    /// This service takes the API information from the WebCrawlerController and applies logic to it.
    /// </summary>
    public class CrawlerService : ICrawlerService
    {
        private readonly HttpClient _httpClient;
        private readonly IHtmlParser _htmlParser;
        private readonly ILogger<CrawlerService> _logger;
        private readonly IHtmlTreeSearch _htmlTreeSearch;

        /// <summary>
        /// This is the CrawlerService constructor. It takes on a few important injected parameters.
        /// </summary>
        /// <param name="httpClientFactory">This factory creates our HTTPClient that we'll be using. It is VERY VERY VERY important that the user agent is similar to one on a browser.</param>
        /// <param name="htmlParser">This class handles logic for parsing HTML.</param>
        /// <param name="logger">This class handles writing errors down.</param>
        /// <param name="htmlTreeSearch">This class handles logic for traversing an HTML tree.</param>
        public CrawlerService(IHttpClientFactory httpClientFactory,
            IHtmlParser htmlParser, 
            ILogger<CrawlerService> logger, 
            IHtmlTreeSearch htmlTreeSearch)
        {
            _httpClient = httpClientFactory.CreateClient();
            // If you get any invalid operation exceptions, switch out the UserAgent.
            // You can get the UserAgent from your browser. This fools Google into thinking it's a normal Google search, not a robot scan.
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0");
            _htmlParser = htmlParser;
            _logger = logger;
            _htmlTreeSearch = htmlTreeSearch;
        }
        /// <summary>
        /// This function first converts the keywords into something that can be sent to Google.
        /// Then, it will send a GET request to Google and return HTML as a string.
        /// </summary>
        /// <param name="keywords">A string array. Example arguments: ["efiling", "integration"]</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">This gets thrown if you insert an empty or null list of keywords. Google can't look up anything without a search query, after all... </exception>
        /// <exception cref="InvalidOperationException">This may get thrown if Google catches you scanning its page. In the event that occurs, please contact gugraves@gmail.com or use the integration tests on files to see that this logic actually works.</exception>
        public async Task<string> GetHtmlContentForKeywordsAsync(IList<string> keywords)
        {
            if (keywords == null || !keywords.Any())
            {
                throw new ArgumentException("Keywords list cannot be null or empty.", nameof(keywords));
            }

            const int ResultsCount = 100;
            string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ResultsCount, keywords);

            try
            {
                string googleHtml = await _httpClient.GetStringAsync(googleUrlWithKeywords);
                return googleHtml;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to fetch HTML content from Google.", ex);
            }
        }
        /// <summary>
        /// This takes a url and tries to find it in the miasma of the Google HTML.
        /// </summary>
        /// <param name="lookupURL">The URL we're trying to find on a Google search result list.</param>
        /// <param name="htmlFromGoogle">The HTML extracted from a GET request on a Google page of 100 results.</param>
        /// <returns>A list of indexes where a link was found, if any were found. E.G. ["1", "2"] </returns>
        public IList<string> ReturnIndexOfGoogleSearchResults(string lookupURL, string htmlFromGoogle)
        {

            List<string> matchingIndexes = [];
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to process search results.");
            }

            return matchingIndexes;
        }
    }
}
