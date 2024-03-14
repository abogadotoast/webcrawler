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
    }
}
