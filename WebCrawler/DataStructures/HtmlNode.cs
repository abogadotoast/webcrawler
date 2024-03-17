namespace WebCrawler.DataStructures
{
    public class HtmlNode(string tagName = "", string content = "", int runningIndex = 0) : IHtmlNode
    {
        public string TagName { get; set; } = tagName;
        public string Content { get; set; } = content;
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public IList<IHtmlNode> Children { get; set; } = [];
        public int RunningIndex { get; set; } = runningIndex;

        public void AddChild(IHtmlNode child)
        {

            if(child != null)
            {
                Children.Add(child);
            }

        }
    }


}
