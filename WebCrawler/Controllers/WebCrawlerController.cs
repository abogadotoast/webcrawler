using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Services;
using WebCrawler.Utilities;

namespace WebCrawler.Controllers
{
    /// <summary>
    /// This is the API layer of the application.
    /// </summary>
    /// <param name="crawlerService">The Crawler Service handles the logic of crawling through HTML and parsing useful information from it.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class WebCrawlerController(ICrawlerService crawlerService) : ControllerBase
    {
        private readonly ICrawlerService _crawlerService = crawlerService;

        /// <summary>
        /// It will return a set of indexes in which the url was found on a Google search.
        /// </summary>
        /// <param name="keywords">Example: ["efiling", "integration"]</param>
        /// <param name="urlToFindOnList">Example: "www.infotrack.com"</param>
        /// <returns>
        /// A list of positions where the URL was found on Google. Starts at 1. 
        /// For example, with those search terms above, I get ["1", "2"]. 
        /// If a site couldn't be found, it will return ["0"].
        /// Note that Google will change the order with its own algorithm - your results may differ from mine.</returns>
        [HttpGet]
        public async Task<ActionResult<IList<string>>> GetAsync([FromQuery] IList<string> keywords, [FromQuery] string urlToFindOnList)
        {
            IList<string> foundUrls = [];

            if (keywords != null && keywords.Count > 0 && !string.IsNullOrWhiteSpace(urlToFindOnList))
            {
                string googleHtml = await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);
                foundUrls = _crawlerService.ReturnIndexOfGoogleSearchResults(urlToFindOnList, googleHtml);

            }
            if(foundUrls.Count == 0)
            {
                foundUrls = ["0"];
            }

            return Ok(foundUrls);
        }
    }
}
