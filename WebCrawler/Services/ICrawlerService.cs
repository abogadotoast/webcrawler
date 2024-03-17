namespace WebCrawler.Services
{
    public interface ICrawlerService
    {
        Task<string> GetHtmlContentForKeywordsAsync(IList<string> keywords);
        IList<string> ReturnIndexOfGoogleSearchResults(string lookupURL, string htmlFromGoogle);
    }

}
