namespace WebCrawler.DataStructures
{
    public interface IHtmlNode : INode
    {
        string TagName { get; set; }
        string Content { get; set; }
        int RunningIndex { get; set; }
        IDictionary<string, string> Attributes { get; set; }
    }

}
