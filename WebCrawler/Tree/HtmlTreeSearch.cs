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
            SearchTreeForAsWithH3(rootNode, matchingNodes, contains);
            return matchingNodes;
        }
        // We still want to check for the nodes that don't match the running count - otherwise we won't be able to increment.
        private void SearchTreeForAsWithH3(IHtmlNode node, IList<IHtmlNode> matchingNodes, string lookupUrl)
        {
            if (node == null) return;

            // Check if the current node is an <a> tag with "href" containing the lookupUrl
            if (node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase) &&
                node.Attributes.TryGetValue("href", out var href) &&
                href.Contains(lookupUrl))
            {
                // Perform a deep search within this <a> tag for an <h3> tag
                if (ContainsH3Tag(node))
                {

                    matchingNodes.Add(node); // Add the <a> node to the list of matches since it contains an <h3>
                }
            }
            else
            {
                // Continue searching deeper if we're not currently processing an <a> tag that matches the criteria
                foreach (var child in node.Children)
                {
                    SearchTreeForAsWithH3(child, matchingNodes, lookupUrl);
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
