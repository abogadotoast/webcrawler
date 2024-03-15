namespace WebCrawler.Tree
{
    public class HtmlNode
    {
        public string TagName { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();

        public HtmlNode(string tagName = "", string content = "")
        {
            TagName = tagName;
            Content = content;
        }

        public void AddChild(HtmlNode child) => Children.Add(child);
    }


}
