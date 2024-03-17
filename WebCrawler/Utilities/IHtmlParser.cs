namespace WebCrawler.Utilities
{
    using System.Collections.Generic;
    using WebCrawler.Tree;

    public interface IHtmlParser
    {
        IHtmlNode ParseHtmlStringIntoTree(string html);
    }

}
