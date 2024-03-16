namespace WebCrawler.Utilities
{
    public class FileOperations : IFileOperations
    {
        public async Task<string> LoadFromFile(string path)
        {
            if (path.Length > 0)
            {
                string readText = await File.ReadAllTextAsync(path);
                return readText;
            }
            return string.Empty;

        }
        public async Task SaveToFile(string text, string path)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteAsync(text);
                }
            }
        }
    }
}
