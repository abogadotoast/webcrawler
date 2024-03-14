using System.Text;

namespace WebCrawler.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// Takes a list of items and combines them into a single string with a "+".
        /// Uses Stringbuilder for higher performance.
        /// </summary>
        /// <param name="items">A list of strings, e.g. ["efiling", "integration"]</param>
        /// <returns>A combined string list for a Google query. For example: "efiling+integration"</returns>
        public static string JoinStringsWithAPlus(List<string> items)
        {
            try
            {
                if (items == null) throw new ArgumentNullException(nameof(items), "Input list cannot be null.");

                if (items.Count == 0) throw new ArgumentException("Input list cannot be empty.", nameof(items));

                StringBuilder sb = new();
                for (int i = 0; i < items.Count; i++)
                {
                    // Here, you might also consider checking for null items within the list
                    // and decide how you want to handle them (ignore, replace, throw, etc.)
                    sb.Append(items[i]);
                    if (i < items.Count - 1)
                    {
                        sb.Append(" + ");
                    }
                }
                return sb.ToString();
            }
            catch (ArgumentNullException ex)
            {
                // Log the exception, rethrow, handle gracefully, etc.
                Console.WriteLine($"Argument exception caught: {ex.Message}");
                throw; // Rethrowing the exception to be handled by the caller.
            }
            catch (ArgumentException ex)
            {
                // Handle other argument exceptions, e.g., empty list
                Console.WriteLine($"Argument exception caught: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// This builds a lookup URL based on the desired number of parameters to return and a list of keywords to lookup from Google.
        /// </summary>
        /// <param name="numberOfSearchResultsToCheck">A number, such as 100. This will ask Google to return 100 records.</param>
        /// <param name="keywords">A list of strings, such as ['efiling', 'integration']. This will be appended at the end of the Google query.</param>
        /// <returns>A URL that queries Google for a number of keywords up to a result list.</returns>
        public static string CreateLookupURL(int numberOfSearchResultsToCheck, List<string> keywords)
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
                string lookupURL = "https://www.google.com/search?num=" + numberOfSearchResultsToCheck + "&q=" + joinedStrings;
                return lookupURL;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Handle or log the ArgumentOutOfRangeException
                // For example, log and then return a default URL or null
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentNullException ex)
            {
                // Handle or log the ArgumentNullException
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (ArgumentException ex)
            {
                // Handle or log the ArgumentException for an empty keywords list
                Console.WriteLine(ex.Message);
                throw;
            }
            // Optionally catch more specific exceptions if JoinStringsWithAPlus could throw them
            catch (Exception ex)
            {
                // Catch-all for any unexpected exceptions
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                throw;
            }
        }
    }
}
