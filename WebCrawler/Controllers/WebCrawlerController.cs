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
        /// It will return a set of indexes in which the url was found on a Google search, as a comma-separated string.
        /// </summary>
        /// <param name="keywords">Example: ["efiling", "integration"]</param>
        /// <param name="urlToFindOnList">Example: "www.infotrack.com"</param>
        /// <returns>
        /// A string of positions where the URL was found on Google, starting at 1, 
        /// or "0" if the site couldn't be found. Note that Google's results may vary.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetAsync([FromQuery] IList<string> keywords, [FromQuery] string urlToFindOnList)
        {
            try
            {
                if (keywords == null || keywords.Count == 0 || string.IsNullOrWhiteSpace(urlToFindOnList))
                {
                    return BadRequest("Keywords and URL to find must be provided.");
                }

                string googleHtml = await _crawlerService.GetHtmlContentForKeywordsAsync(keywords);
                IList<string> indicesOfFoundUrls = _crawlerService.ReturnIndexOfGoogleSearchResults(urlToFindOnList, googleHtml);

                string foundUrls = "0";
                if (indicesOfFoundUrls.Count > 0)
                {
                    // Convert all indices to string and concatenate them, separated by commas
                    foundUrls = string.Join(", ", indicesOfFoundUrls.Select(index => index).ToArray());
                }

                return Ok(foundUrls);
            }
            catch
            {
                // Log the exception details as needed
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

    }
}
