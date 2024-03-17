using WebCrawler.Services;

namespace WebCrawler.Utilities
{
    /// <summary>
    /// This class handles all of the file operations.
    /// </summary>
    /// <param name="logger">A logger to record errors.</param>
    public class FileOperations(ILogger<FileOperations> logger) : IFileOperations
    {
        private readonly ILogger<FileOperations> _logger = logger;

        /// <summary>
        /// This loads a file and converts it to text.
        /// </summary>
        /// <param name="path">The path where that file can be found.</param>
        /// <returns>The text of that file.</returns>
        public async Task<string> LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogWarning("Attempted to load from a file with an empty or null path.");
                return string.Empty;
            }

            try
            {
                string readText = await File.ReadAllTextAsync(path);
                return readText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading content from the file at {Path}.", path);
                return string.Empty;
            }
        }

        /// <summary>
        /// This saves a file with the desired path.
        /// </summary>
        /// <param name="text">This is the text you want to save.</param>
        /// <param name="path">This is the path in which you want to save it.</param>
        /// <returns></returns>
        public async Task SaveToFile(string text, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogWarning("Attempted to save to a file with an empty or null path.");
                return;
            }

            try
            {
                // Check if the file exists to decide on logging the action
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        await sw.WriteAsync(text);
                    }
                    _logger.LogInformation("A new file was created and text was written to {Path}.", path);
                }
                else
                {
                    _logger.LogInformation("The file {Path} already exists. No new file was created.", path);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving content to the file at {Path}.", path);
            }
        }
    }
}
