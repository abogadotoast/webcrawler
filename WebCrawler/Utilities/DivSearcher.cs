using System.Net.Http;
using System.Text.RegularExpressions;

namespace WebCrawler.Utilities
{
    public class DivSearcher
    {
        public static List<bool> FindWordInDivChildren(string htmlContent)
        {
            // Extract the main 'rso' div content.
            string rsoDivContent = ExtractDivContentById(htmlContent, "search");

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
        public static bool FindSpanWithText(string htmlContent, string searchText)
        {
            // Escaping the search text for use in a regular expression pattern

            // Regular expression to find a <span> containing the specified searchText
            // This pattern assumes that the searchText is directly within the span and does not span across multiple nested elements
            string pattern = $@"<span[^>]*>\s*[^<]*{searchText}[^<]*\s*<\/span>";

            // Performing the search
            bool containsText = Regex.IsMatch(htmlContent, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return containsText;
        }
        public static int CountWordOccurrences(string htmlContent, string word)
        {
            // First, remove script and style sections to avoid counting words in scripts or styles
            string scriptPattern = @"<script[^>]*>[\s\S]*?</script>";
            string stylePattern = @"<style[^>]*>[\s\S]*?</style>";
            htmlContent = Regex.Replace(htmlContent, scriptPattern, "", RegexOptions.IgnoreCase);
            htmlContent = Regex.Replace(htmlContent, stylePattern, "", RegexOptions.IgnoreCase);

            // Remove HTML tags to extract text content
            string htmlTagPattern = @"<[^>]+>";
            string textContent = Regex.Replace(htmlContent, htmlTagPattern, " ");

            // Escape special characters in the word to search
            string escapedWord = Regex.Escape(word);

            // Use a regex to count occurrences of the word, considering word boundaries to ensure complete words are matched
            string wordPattern = $@"\b{escapedWord}\b";
            MatchCollection matches = Regex.Matches(textContent, wordPattern, RegexOptions.IgnoreCase);

            return matches.Count;
        }

        public static string GetSearchResultsPositionAsync(string htmlContent, string targetUrl)
        {

            // Look for the div with id="rso"
            var rsoDivPattern = @"<div[^>]+id=[""']?rso[""']?[^>]*>(.*?)<\/div>";
            var rsoMatch = Regex.Match(htmlContent, rsoDivPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!rsoMatch.Success) return "0";

            // Within "rso", search for divs with class="MjjYud"
            var resultsPattern = $@"<div[^>]+class=[""']?MjjYud[""']?[^>]*>.*?<a href=[""']?(https?://[^'"" >]+)[""']?.*?</div>";
            var matches = Regex.Matches(rsoMatch.Groups[1].Value, resultsPattern, RegexOptions.IgnoreCase);

            List<int> positions = new List<int>();
            int currentPosition = 1;

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups[1].Value.Contains(targetUrl))
                {
                    positions.Add(currentPosition);
                }
                currentPosition++;
            }

            return positions.Count > 0 ? string.Join(", ", positions) : "0";
        }
    }
}
