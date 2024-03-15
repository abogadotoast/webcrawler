namespace WebCrawler.Tree
{
    public interface IHtmlNode
    {
        string TagName { get; set; }
        string Content { get; set; }
        int RunningIndex { get; set; }
        IDictionary<string, string> Attributes { get; set; }
        IList<IHtmlNode> Children { get; set; }
        string Path { get; set; }
        // Other properties and methods common to all nodes
        void AddChild(HtmlNode child);
    }

}
