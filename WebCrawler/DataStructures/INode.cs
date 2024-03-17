namespace WebCrawler.DataStructures
{
    public interface INode
    {
        void AddChild(IHtmlNode child);
        IList<IHtmlNode> Children { get; set; }
    }
}
