using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebCrawler.Services
{
    public class HtmlParser : IHtmlParser
    {
        private string? _htmlContent;
        private string? _searchWord;

        public IHtmlParser LoadHtml(string htmlContent)
        {
            _htmlContent = htmlContent;
            return this;
        }

        public IHtmlParser FindWord(string word)
        {
            _searchWord = word;
            return this;
        }

        public IEnumerable<string> InH3Tags()
        {
            if (string.IsNullOrEmpty(_htmlContent))
                throw new InvalidOperationException("HTML content has not been loaded.");

            if (string.IsNullOrEmpty(_searchWord))
                throw new InvalidOperationException("Search word has not been specified.");

            var matches = new List<string>();

            // Regular expression to match <h3> tags. This is a simple approach and may not handle all HTML cases.
            var regex = new Regex(@"<h3.*?>(.*?)<\/h3>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matchCollection = regex.Matches(_htmlContent);

            foreach (Match match in matchCollection)
            {
                var h3Content = match.Groups[1].Value; // The inner content of the h3 tag
                if (h3Content.Contains(_searchWord, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(h3Content);
                }
            }

            return matches;
        }
        public bool WordExistsInAllSubDivsOf(string divId)
        {
            // Escape the word for use in a regex pattern to avoid issues with special characters
            string escapedWord = Regex.Escape(_searchWord);

            // Dynamically build the regex pattern to match the specified div by its ID
            string divPattern = $"<div id='{divId}'>(.*?)</div>";
            var match = Regex.Match(_htmlContent, divPattern, RegexOptions.Singleline);

            if (!match.Success) return false; // Specified div not found

            // Get the first matching group.
            string divContent = match.Groups[0].Value;

            // Find all sub-divs within the specified div
            var subDivsPattern = @"<div(.*?)</div>";
            var subDivMatches = Regex.Matches(divContent, subDivsPattern, RegexOptions.Singleline);

            if (subDivMatches.Count == 0) return false; // No sub-divs found

            foreach (Match subDivMatch in subDivMatches)
            {
                // Check if the word exists in the current sub-div
                if (!Regex.IsMatch(subDivMatch.Value, escapedWord, RegexOptions.IgnoreCase))
                {
                    return false; // Word not found in this sub-div
                }
            }

            return true; // Word found in all sub-divs
        }
        public static List<bool> FindWordInDivChildren(string htmlContent)
        {
            // Extract the main 'rso' div content.
            string rsoDivContent = ExtractDivContentById(htmlContent, "rso");

            // Initialize the list to store results for each child div.
            var results = new List<bool>();

            // Recursively search each child div for the word.
            SearchDivChildrenForWord(rsoDivContent, "Infotrak", results);

            return results;
        }

        private static void SearchDivChildrenForWord(string divContent, string word, List<bool> results)
        {
            // Use a non-greedy regex to match the first level of child divs.
            var childDivRegex = new Regex(@"<div[^>]*>(.*?)<\/div>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Find all child divs at the current level.
            var matches = childDivRegex.Matches(divContent);

            foreach (Match match in matches)
            {
                string childContent = match.Groups[1].Value;

                // Check if the word exists in the current child div.
                bool found = Regex.IsMatch(childContent, @"\b" + Regex.Escape(word) + @"\b", RegexOptions.IgnoreCase);

                // Add the result for the current child div.
                results.Add(found);

                // Recursively search in the current child div for more nested divs.
                SearchDivChildrenForWord(childContent, word, results);
            }
        }

        private static string ExtractDivContentById(string htmlContent, string id)
        {
            var divRegex = new Regex($@"<div[^>]*id=['""]?{id}['""]?[^>]*>(.*?)<\/div>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var match = divRegex.Match(htmlContent);

            if (match.Success)
            {
                // Return the content of the matched div.
                return match.Groups[1].Value;
            }

            // Return an empty string if the div with the specified id is not found.
            return string.Empty;
        }
    }
}
