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
        /// <summary>
        /// This takes a list of keywords and a string URL.
        /// It then will return all positions where those keywords are found on a Google URL.
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="URL"></param>
        /// <returns></returns>
        
        public async Task<List<string>> ReturnIndexOfGoogleSearchResults(List<string> keywords, string URL, string numberOfSearchResultsToCheck)
        {
            try
            {
              //  numberOfSearchResultsToCheck = "100";
              //  keywords = ["efiling", "integration"];
                
                List<string> matchingIndexes = [];
                
                

                // Google might block requests that do not seem to originate from a browser,
                // so setting a user-agent that mimics a browser can help.
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");

                string html = await _httpClient.GetStringAsync(URL);

                var parser = new SimpleHtmlParser();
                var rootNode = parser.ParseHtml(html);
                var treeSearch = new HtmlTreeSearch();
                //query:efiling%20integration
            //    var allNodes = treeSearch.FindDivsWithDataAsyncContext(rootNode, "https://www.infotrack.com", @"/url?q=");
                var matchingNodes = treeSearch.FindDivsWithDataAsyncContext(rootNode, "https://www.infotrack.com", @"/url?q=https://www.infotrack.com");
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
            return null;
        }



    }
}
