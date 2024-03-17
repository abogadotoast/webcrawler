using System;
using System.IO;

namespace WebCrawler.Tree
{
    public class HtmlTreeSearch
    {
        // Function to search the HTML tree for <a> tags with specific "href" attributes and no content
        public List<IHtmlNode> FindDivsWithDataAsyncContext(IHtmlNode rootNode, string contains)
        {
            var matchingNodes = new List<IHtmlNode>();
            int runningIndex = 0; // Initialize runningIndex
            SearchTreeForAsWithH3(rootNode, matchingNodes, contains, ref runningIndex);
            return matchingNodes;
        }
        // We still want to check for the nodes that don't match the running count - otherwise we won't be able to increment.
        private void SearchTreeForAsWithH3(IHtmlNode node, IList<IHtmlNode> matchingNodes, string lookupUrl, ref int runningIndex)
        {
            if (node == null) return;

            // First, let's increment without caring about the contains.
            // Then, set the node's current index.
            if (node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase) &&
                node.Attributes.TryGetValue("href", out var href))
            {
                if (ContainsH3Tag(node))
                {
                    runningIndex++;
                    if (href.Contains(lookupUrl))
                    {
                        {
                            node.RunningIndex = runningIndex;
                            matchingNodes.Add(node);
                        }
                    }
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    SearchTreeForAsWithH3(child, matchingNodes, lookupUrl, ref runningIndex);
                }
            }
        }
        // Helper method to check for <h3> tag existence within an <a> tag
        private bool ContainsH3Tag(IHtmlNode node)
        {
            if (node.TagName.Equals("h3", StringComparison.CurrentCultureIgnoreCase))
            {
                return true; // Found an <h3> tag
            }

            foreach (var child in node.Children)
            {
                if (ContainsH3Tag(child))
                {
                    return true; // Found an <h3> tag in descendants
                }
            }

            return false; // No <h3> tag found
        }

    }

}
