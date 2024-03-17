using WebCrawler.Services;

namespace WebCrawler.Utilities
{
    public class FileOperations : IFileOperations
    {
        ILogger<FileOperations> _logger;
        public FileOperations(ILogger<FileOperations> logger)
        {
            _logger = logger;
        }
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
                _logger.LogError(ex, $"An error occurred while loading content from the file at {path}.");
                return string.Empty;
            }
        }


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
                    _logger.LogInformation($"A new file was created and text was written to {path}.");
                }
                else
                {
                    _logger.LogInformation($"The file {path} already exists. No new file was created.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while saving content to the file at {path}.");
            }
        }
    }
}
