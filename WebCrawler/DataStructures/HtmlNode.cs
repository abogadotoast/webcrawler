namespace WebCrawler.DataStructures
{
    /// <summary>
    /// This is a Node. It is part of the Tree data structure.
    /// </summary>
    /// <param name="tagName">This is the HTML tag, e.g. "a" or "div" or "p".</param>
    /// <param name="content">Some tags have text within them.</param>
    /// <param name="runningIndex">As we propagate through the tree, we want to remember where we found the link on the result set. This records that information.</param>
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
