namespace WebCrawler.Tree
{
    public class HtmlNode
    {
        public string TagName { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();
        public HtmlNode Parent { get; private set; } // Reference to the parent node
        public int InsertionIndex { get; set; }
        public int RunningIndex { get; set; }


        public HtmlNode(string tagName = "", string content = "", string path = "", HtmlNode parent = null)
        {
            TagName = tagName;
            Content = content;
            Path = path;
            Parent = parent; // Initialize parent
        }

        public void AddChild(HtmlNode child)
        {

            if(child != null)
            {
                // Set the child's InsertionIndex as the next index in the Children list
                child.InsertionIndex = Children.Count;
                // Update child's parent to this node
                child.Parent = this;
                Children.Add(child);
            }

        }
        public int GetSiblingCount()
        {
            // If the node has no parent, it's considered as the root, so no siblings
            if (this.Parent == null) return 0;

            // Return the count of the parent's children minus one (excluding the current node itself)
            return this.Parent.Children.Count - 1;
        }
    }


}
