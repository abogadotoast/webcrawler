using System;
using System.Text.RegularExpressions;
using WebCrawler.Services;
using WebCrawler.Tree;
using WebCrawler.Utilities;

namespace WebCrawler.Services
{
    public class CrawlerService
    {
        private readonly HttpClient _httpClient;
        private readonly IHtmlParser _htmlParser;
        public CrawlerService(IHttpClientFactory httpClientFactory, IHtmlParser htmlParser)
        {
            _httpClient = httpClientFactory.CreateClient();
            _htmlParser = htmlParser;
        }
        public async Task<IList<string>> ReturnIndexOfGoogleSearchResults(string URL)
        {
            List<string> matchingIndexes = [];
            try
            {
                // Google might block requests that do not seem to originate from a browser,
                // so setting a user-agent that mimics a browser can help.
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");

                string html = await _httpClient.GetStringAsync(URL);

                var rootNode = _htmlParser.ParseHtml(html);
                var treeSearch = new HtmlTreeSearch();

                var matchingNodes = treeSearch.FindDivsWithDataAsyncContext(rootNode, @"/url?q=https://www.infotrack.com");
                // Iterate through each of the nodes to get the indexes.
                foreach( var node in matchingNodes)
                {
                    matchingIndexes.Add(node.RunningIndex.ToString());
                }
                return matchingIndexes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            // If no matches, return an empty array.
            return matchingIndexes;
        }



    }
}
