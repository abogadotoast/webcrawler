namespace WebCrawler.Services
{
    using System.Collections.Generic;

    public interface IHtmlParser
    {
        IHtmlParser LoadHtml(string html);
        IHtmlParser FindWord(string word);
        IEnumerable<string> InH3Tags();
    }

}
