using System;
using System.Text.RegularExpressions;
using WebCrawler.Services;
using WebCrawler.Tree;
using WebCrawler.Utilities;
using static System.Net.Mime.MediaTypeNames;
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
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0");
            _htmlParser = htmlParser;
            _logger = logger;
            _htmlTreeSearch = htmlTreeSearch;
        }
        public async Task PrintHtmlToFile(string text)
        {
            string path = @"C:\Users\gugra\source\repos\WebCrawlerProj\WebCrawler\html\latestHtmlFile.html";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteAsync(text);
                }
            }
        }
        public async Task<string> LoadHtmlFromFile()
        {
            string path = @"C:\Users\gugra\source\repos\WebCrawlerProj\WebCrawler\html\latestHtmlFile.html";
            string readText = await File.ReadAllTextAsync(path);
            return readText;
        }
        public async Task<IList<string>> ReturnIndexOfGoogleSearchResults(string lookupURL, IList<string> keywords)
        {
            const int ONE_HUNDRED_RESULTS_FROM_GOOGLE = 100;
            List<string> matchingIndexes = new List<string>();

            try
            {
               // string appendedLookupURL = StringUtilities.AppendUrlAndQToWebsiteBeingSearched(lookupURL);
                string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ONE_HUNDRED_RESULTS_FROM_GOOGLE, keywords);
                string html = await LoadHtmlFromFile();
                if(html == null)
                {
                    // For testing purposes, we're going to load the same file from text.
                    // Google is annoying and changes their page if it's obvious we're scraping.
                    html = await _httpClient.GetStringAsync(googleUrlWithKeywords);
                    await PrintHtmlToFile(html);
                }
                var rootNode = _htmlParser.ParseHtmlStringIntoTree(html);
                var matches = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, lookupURL);
               // var matchingNodes = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, lookupURL);

                //  matchingIndexes.AddRange(matchingNodes.Select(node => node.RunningIndex.ToString()));
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
