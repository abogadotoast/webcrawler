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
            int runningIndex = 0; // Reset running index for each search
            SearchTree(rootNode, matchingNodes, contains, ref runningIndex);
            return matchingNodes;
        }

        // Recursive function to traverse the tree
        private void SearchTree(IHtmlNode node, IList<IHtmlNode> matchingNodes, string contains, ref int runningIndex, string path = "", int childIndex = 0)
        {
            const string URL_PARAMETER_FROM_GOOGLE_LIST = @"url?q=";
            if (node == null) return; // Early return for null nodes

            // Update path for current node
            string currentPath = path + (string.IsNullOrEmpty(path) ? "" : ".") + childIndex;

            // Check if the node matches the search criteria
            if (IsMatchingNode(node, contains))
            {
                node.Path = currentPath; // Store the path directly in the node
                node.RunningIndex = ++runningIndex; // Increment and set running index - match the path
                matchingNodes.Add(node);
            }
            else if (IsMatchingNode(node, URL_PARAMETER_FROM_GOOGLE_LIST)){
                ++runningIndex; // START at 1
            }

            // Recursively search in child nodes
            for (int i = 0; i < node.Children.Count; i++)
            {
                SearchTree(node.Children[i], matchingNodes, contains, ref runningIndex, currentPath, i);
            }
        }

        // Check if the node matches the search criteria
        private bool IsMatchingNode(IHtmlNode node, string contains)
        {
            return node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase) &&
                   node.Attributes.TryGetValue("href", out var href) &&
                   href.Contains(contains) &&
                   string.IsNullOrEmpty(node.Content); // Only Google puts Content in this tree
        }
        // We still want to check for the nodes that don't match the running count - otherwise we won't be able to increment.
    }

}
