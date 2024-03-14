using System;
using System.Text.RegularExpressions;

namespace WebCrawler.Services
{
    public class CrawlerService
    {
        private readonly HttpClient _httpClient;
        public CrawlerService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        /// <summary>
        /// This takes a list of keywords and a string URL.
        /// It then will return all positions where those keywords are found on a Google URL.
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="URL"></param>
        /// <returns></returns>
        
        private async Task<List<string>> ReturnIndexOfGoogleSearchResults(List<string> keywords, string URL, string numberOfSearchResultsToCheck)
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

                // A simple regex to match 'www.infotrack.com' within anchor tags. This is a very basic approach
                // and might need adjustments based on actual HTML structure and URL formats.
                var matches = Regex.Matches(html, @"href=""(https?://www\.infotrack\.com[^""]*)""", RegexOptions.IgnoreCase);

                int index = 1; // Starting index at 1 for human-readable indexing
                foreach (Match match in matches)
                {
                    Console.WriteLine($"Instance found at index {index}: {match.Groups[1].Value}");
                    index++;
                    matchingIndexes.Add(index.ToString());
                }

                if (index == 1)
                {
                    Console.WriteLine("No instances of 'www.infotrack.com' were found.");
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
