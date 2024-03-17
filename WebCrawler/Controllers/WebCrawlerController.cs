using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Services;
using WebCrawler.Utilities;

namespace WebCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebCrawlerController : ControllerBase
    {
        CrawlerService _crawlerService;
        public WebCrawlerController(CrawlerService crawlerService)
        {
            _crawlerService = crawlerService;
        }
        // GET: api/WebCrawler
        [HttpGet]
        public async Task<ActionResult<IList<string>>> GetAsync([FromQuery] IList<string> keywords, [FromQuery] string urlToFindOnList)
        {
            // Placeholder for the actual web crawling logic to find the URL in the list based on the keywords
            IList<string> foundUrls = new List<string>();

            // Example logic: add the URL to the list if a specific dummy keyword is found.
            // Replace this with your actual logic for web crawling and finding URLs based on keywords.
            if (keywords != null && keywords.Count > 0 && !string.IsNullOrWhiteSpace(urlToFindOnList))
            {
                string googleHtml = await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);
                foundUrls = _crawlerService.ReturnIndexOfGoogleSearchResults(urlToFindOnList, googleHtml);

            }

            return Ok(foundUrls);
        }
    }
}
