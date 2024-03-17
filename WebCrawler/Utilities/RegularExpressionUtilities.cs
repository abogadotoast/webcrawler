namespace WebCrawler.Utilities
{
    using System.Text.RegularExpressions;
    using System.Text.RegularExpressions.Generated;

    public static partial class RegexUtilities
    {
        [GeneratedRegex(@"(</?[^>]+>)|([^<]+)")]
        public static partial Regex CreateHtmlTagRegex();

        [GeneratedRegex(@"(\w+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?")]
        public static partial Regex HTMLAttributeMatcher();
    }

}
