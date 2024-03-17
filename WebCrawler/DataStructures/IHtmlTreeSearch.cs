namespace WebCrawler.DataStructures
{
    public interface IHtmlTreeSearch
    {
        List<IHtmlNode> FindDivsWithDataAsyncContext(IHtmlNode rootNode, string contains);
    }

}
