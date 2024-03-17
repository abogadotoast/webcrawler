using System;
using System.IO;

namespace WebCrawler.DataStructures
{
    /// <summary>
    /// This class contains methods to find objects within a tree of HTML objects.
    /// </summary>
    /// <param name="logger">A logger to record errors.</param>
    public class HtmlTreeSearch(ILogger<HtmlTreeSearch> logger) : IHtmlTreeSearch
    {
        private readonly ILogger<HtmlTreeSearch> _logger = logger;

        /// <summary>
        /// This function acts as our entry point into the tree, setting up the initial instructions and objects.
        /// </summary>
        /// <param name="rootNode">The node we're currently on.</param>
        /// <param name="url">The url being searched.</param>
        /// <returns>If there's a match, it'll return a list of nodes that match the url being searched.</returns>
        public List<IHtmlNode> FindDivsWithDataAsyncContext(IHtmlNode rootNode, string url)
        {
            var matchingNodes = new List<IHtmlNode>();
            try
            {
                int runningIndex = 0; // Initialize runningIndex
                SearchTreeForAsWithH3(rootNode, matchingNodes, url, ref runningIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching the HTML tree.");
                throw;
            }
            return matchingNodes;
        }
        /// <summary>
        /// This function looks for an a href tag that embeds an H3, somewhere inside of it. 
        /// This handles our tree traversal.
        /// It also handles counting where the match was found on a list of objects.
        /// </summary>
        /// <param name="node">The current node on the tree.</param>
        /// <param name="matchingNodes">All of the nodes that match what we're looking for.</param>
        /// <param name="url">The URL that we're looking for in Google's obfuscated HTML tree.</param>
        /// <param name="runningIndex">The amount of matches we've found.</param>
        private void SearchTreeForAsWithH3(IHtmlNode node, IList<IHtmlNode> matchingNodes, string url, ref int runningIndex)
        {
            try
            {
                if (node == null) return;

                if (node.TagName.Equals("a", StringComparison.CurrentCultureIgnoreCase) &&
                    node.Attributes.TryGetValue("href", out var href))
                {
                    if (ContainsH3Tag(node))
                    {
                        runningIndex++;
                        if (href.Contains(url))
                        {
                            node.RunningIndex = runningIndex;
                            matchingNodes.Add(node);
                        }
                    }
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        SearchTreeForAsWithH3(child, matchingNodes, url, ref runningIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during recursive search
                Console.WriteLine($"An error occurred while processing a node: {ex.Message}");
            }
        }
        /// <summary>
        /// This function looks for h3 tags nested inside of Google's list of search results. We use this to anchor our tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>Whether or not there's an H3 tag here or in any of the child objects.</returns>
        private bool ContainsH3Tag(IHtmlNode node)
        {
            try
            {
                if (node.TagName.Equals("h3", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }

                foreach (var child in node.Children)
                {
                    if (ContainsH3Tag(child))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking for H3 tags.");
                return false;
            }
            return false;
        }
    }


}
