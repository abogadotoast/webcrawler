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
    }
}
