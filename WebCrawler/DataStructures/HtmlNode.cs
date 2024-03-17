namespace WebCrawler.DataStructures
{
    public class HtmlNode : IHtmlNode
    {
        public string TagName { get; set; }
        public string Content { get; set; }
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public IList<IHtmlNode> Children { get; set; } = new List<IHtmlNode>();
        public int RunningIndex { get; set; }


        public HtmlNode(string tagName = "", string content = "", int runningIndex = 0)
        {
            TagName = tagName;
            Content = content;
            RunningIndex = runningIndex;
        }

        public void AddChild(HtmlNode child)
        {

            if(child != null)
            {
                Children.Add(child);
            }

        }
    }


}
