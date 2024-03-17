namespace WebCrawler.DataStructures
{
    public interface INode
    {
        void AddChild(HtmlNode child);
        IList<IHtmlNode> Children { get; set; }
    }
}
