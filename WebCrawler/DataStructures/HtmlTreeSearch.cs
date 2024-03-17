using System;
using System.IO;

namespace WebCrawler.DataStructures
{
    public class HtmlTreeSearch(ILogger<HtmlTreeSearch> logger) : IHtmlTreeSearch
    {
        private readonly ILogger<HtmlTreeSearch> _logger = logger;

        public List<IHtmlNode> FindDivsWithDataAsyncContext(IHtmlNode rootNode, string contains)
        {
            var matchingNodes = new List<IHtmlNode>();
            try
            {
                int runningIndex = 0; // Initialize runningIndex
                SearchTreeForAsWithH3(rootNode, matchingNodes, contains, ref runningIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching the HTML tree.");
                throw; // Rethrow the exception if you cannot handle it adequately here
            }
            return matchingNodes;
        }

        private void SearchTreeForAsWithH3(IHtmlNode node, IList<IHtmlNode> matchingNodes, string lookupUrl, ref int runningIndex)
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
                        if (href.Contains(lookupUrl))
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
                        SearchTreeForAsWithH3(child, matchingNodes, lookupUrl, ref runningIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during recursive search
                Console.WriteLine($"An error occurred while processing a node: {ex.Message}");
                // Depending on your error handling policy, you may continue, return, or rethrow.
            }
        }

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
