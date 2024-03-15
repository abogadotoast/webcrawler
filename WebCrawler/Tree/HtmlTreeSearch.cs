using System;
using System.IO;

namespace WebCrawler.Tree
{
    public class HtmlTreeSearch
    {
        private int runningIndex = 0; // Global running index
        // Function to search the HTML tree
        public List<IHtmlNode> FindDivsWithDataAsyncContext(IHtmlNode rootNode, string contains)
        {
            var matchingNodes = new List<IHtmlNode>();
            SearchTree(rootNode, matchingNodes, contains);
            return matchingNodes;
        }

        // Recursive function to traverse the tree
        private void SearchTree(IHtmlNode node, List<IHtmlNode> matchingNodes, string contains, string path = "")
        {
            // Construct the path for the current node
            string currentPath = string.IsNullOrEmpty(path) ? "" : path + ".";
            // We get the nodes that have an <a href="/url?q={URL_HERE}>.
            // This is what I determined as the "secret key" of each of the 100 listings.
            // The ones we are interested in do not have Content - only the Google provided links appear to have Content.

            if( node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase)
                && node.Attributes["href"].Contains(@"url?q=")
                & string.IsNullOrEmpty(node.Content)) // Only Google puts Content in this tree.)
            {
                // Start at 1, since we want to return a human countable index.
                ++runningIndex;
            }


            if (node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase)
                     //@"/url?q="
                     && node.Attributes["href"].Contains(contains)
                     && string.IsNullOrEmpty(node.Content)) // Only Google puts Content in this tree.
            {
                node.Path = path; // Optional: Store the path directly in the node
                node.RunningIndex = runningIndex;
                matchingNodes.Add(node);
            }


            // Recursively search in child nodes, updating the path for each child
            for (int i = 0; i < node.Children.Count; i++)
            {
                string childPath = currentPath + i; // Construct the child's path
                SearchTree(node.Children[i], matchingNodes, contains, childPath);
            }
        }

    }
}
