namespace WebCrawler.Utilities
{
    public interface IFileOperations
    {
        Task<string> LoadFromFile(string path);
        Task SaveToFile(string text, string path);
    }

}
