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
        public async Task PrintHtmlToFile(string text, string path)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteAsync(text);
                }
            }
        }
        public async Task<string> LoadHtmlFromFile(string path)
        {
            if (path.Length > 0)
            {
                string readText = await File.ReadAllTextAsync(path);
                return readText;
            }
            return string.Empty;

        }
        public async Task<string> CreateHTMLFileForParsing(IList<string> keywords, bool isWebEnabled)
        {
            const int ONE_HUNDRED_RESULTS_FROM_GOOGLE = 100;
            string googleUrlWithKeywords = StringUtilities.CreateLookupURL(ONE_HUNDRED_RESULTS_FROM_GOOGLE, keywords);
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            string path = Path.Combine(projectDirectory, @"Services\CrawlerServiceFunctions\html\", "latestHtmlFile.html");
            string html = await LoadHtmlFromFile(path);
            if (html == null || isWebEnabled)
            {
                // For testing purposes, we're going to load the same file from text.
                // Google is annoying and changes their page if it's obvious we're scraping.
                html = await _httpClient.GetStringAsync(googleUrlWithKeywords);
            }
            return html;
        }
        public async Task<IList<string>> ReturnIndexOfGoogleSearchResults(string lookupURL, string htmlFromGoogle)
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
