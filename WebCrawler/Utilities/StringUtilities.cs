using System.Text;
using System.Web;

namespace WebCrawler.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// Takes a list of strings and safely combines them with UrlEncoding.
        /// </summary>
        /// <param name="items">A list of strings, e.g. ["efiling", "integration"]</param>
        /// <returns>A combined string list for a Google query. For example: "efiling+integration"</returns>
        public static string JoinStringsWithAPlus(IList<string>? keywords)
        {
            if (keywords != null)
            {
                return string.Join("+", keywords.Select(HttpUtility.UrlEncode));
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// This builds a lookup URL based on the desired number of parameters to return and a list of keywords to lookup from Google.
        /// </summary>
        /// <param name="numberOfSearchResultsToCheck">A number, such as 100. This will ask Google to return 100 records.</param>
        /// <param name="keywords">A list of strings, such as ['efiling', 'integration']. This will be appended at the end of the Google query.</param>
        /// <returns>A URL that queries Google for a number of keywords up to a result list.</returns>
        public static string CreateLookupURL(int numberOfSearchResultsToCheck, IList<string> keywords)
        {
            try
            {
                if (numberOfSearchResultsToCheck < 1 || numberOfSearchResultsToCheck > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(numberOfSearchResultsToCheck), "Number of search results must be between 1 and 100.");
                }

                if (keywords == null)
                {
                    throw new ArgumentNullException(nameof(keywords), "Keywords list cannot be null.");
                }

                if (!keywords.Any())
                {
                    throw new ArgumentException("Keywords list cannot be empty.", nameof(keywords));
                }

                string joinedStrings = JoinStringsWithAPlus(keywords);
                string lookupURL = $"https://www.google.com/search?num={numberOfSearchResultsToCheck}&q={joinedStrings}";
                return lookupURL;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                throw;
            }
        }
        public static string AppendUrlAndQToWebsiteBeingSearched(string websiteToLookup)
        {
            return @"/url?q=" + websiteToLookup;
        }
    }
}
