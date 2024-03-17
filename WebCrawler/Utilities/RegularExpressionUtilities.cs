namespace WebCrawler.Utilities
{
    using System.Text.RegularExpressions;
    using System.Text.RegularExpressions.Generated;

    /// <summary>
    /// This is a new feature from .NET 7 that efficiently compiles RegularExpressions before runtime.
    /// https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-source-generators
    /// </summary>
    public static partial class RegexUtilities
    {
        [GeneratedRegex(@"(</?[^>]+>)|([^<]+)")]
        public static partial Regex CreateHtmlTagRegex();

        [GeneratedRegex(@"(\w+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?")]
        public static partial Regex HTMLAttributeMatcher();
    }

}
