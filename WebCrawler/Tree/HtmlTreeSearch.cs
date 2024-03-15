namespace WebCrawler.Tree
{
public class HtmlTreeSearch
{
    // Function to search the HTML tree
    public List<HtmlNode> FindDivsWithDataAsyncContext(HtmlNode rootNode, string contextValue)
    {
        var matchingNodes = new List<HtmlNode>();
        SearchTree(rootNode, contextValue, matchingNodes);
        return matchingNodes;
    }

    // Recursive function to traverse the tree
    private void SearchTree(HtmlNode node, string contextValue, List<HtmlNode> matchingNodes)
    {
        // Check if the current node is a div with the specified data-async-context attribute
      if (node.TagName.ToLower() == "a"
               && node.Attributes["href"].Contains(@"/url?q=" + contextValue))
            // node.Attributes.ContainsKey("data-async-context"))
            // && node.Attributes["data-async-context"] == contextValue)
            {

            matchingNodes.Add(node);
        }

        // Recursively search in child nodes
        foreach (var child in node.Children)
        {
            SearchTree(child, contextValue, matchingNodes);
        }
    }
}
}
