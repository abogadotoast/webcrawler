namespace WebCrawler.Services
{
    using System.Collections.Generic;
    using WebCrawler.Tree;

    public interface IHtmlParser
    {
        IHtmlNode ParseHtml(string html);
    }

}
