using System;
using System.IO;

namespace WebCrawler.Tree
{
    public class HtmlTreeSearch
    {
        private int runningIndex = 0; // Global running index
        // Function to search the HTML tree
        public List<HtmlNode> FindDivsWithDataAsyncContext(HtmlNode rootNode, string contextValue, string contains)
        {
            var matchingNodes = new List<HtmlNode>();
            SearchTree(rootNode, contextValue, matchingNodes, contains);
            return matchingNodes;
        }

        // Recursive function to traverse the tree
        private void SearchTree(HtmlNode node, string contextValue, List<HtmlNode> matchingNodes, string contains, string path = "")
        {
            // Construct the path for the current node
            string currentPath = string.IsNullOrEmpty(path) ? "" : path + ".";
            // Increment the index.

            // Preserve the original index.
            // Check if the current node is a div with the specified data-async-context attribute

            // We increment the running index based on the number of <a href="" we find that have url?q =
            // This minimizes it to hopefully just the 100 search results
            if( node.TagName.ToLower() == "a"
                && node.Attributes["href"].Contains(@"url?q=")
                & string.IsNullOrEmpty(node.Content)) // Only Google puts Content in this tree.)
            {
                // Start at 1, since we want to return a human countable index.
                ++runningIndex;
            }
            if (node.TagName.ToLower() == "a"
                     //@"/url?q="
                     && node.Attributes["href"].Contains(contains)
                     && string.IsNullOrEmpty(node.Content)) // Only Google puts Content in this tree.
            // node.Attributes.ContainsKey("data-async-context"))
            // && node.Attributes["data-async-context"] == contextValue)
            {
                node.Path = path; // Optional: Store the path directly in the node
                node.RunningIndex = runningIndex;
                matchingNodes.Add(node);
            }


            // Recursively search in child nodes, updating the path for each child
            for (int i = 0; i < node.Children.Count; i++)
            {
                string childPath = currentPath + i; // Construct the child's path
                SearchTree(node.Children[i], contextValue, matchingNodes, contains, childPath);
            }
        }
    }
}
