namespace WebCrawler.Utilities
{
    using System.Collections.Generic;
    using WebCrawler.DataStructures;

    public interface IHtmlParser
    {
        IHtmlNode ParseHtmlStringIntoTree(string html);
    }

}
