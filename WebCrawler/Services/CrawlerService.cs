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

    }
}
