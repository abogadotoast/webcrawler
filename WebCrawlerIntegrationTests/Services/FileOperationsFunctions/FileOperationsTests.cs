using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.Services.FileOperationsFunctions
{
    [TestClass]
    public class FileOperationsTests
    {
        private static IServiceProvider? _serviceProvider;
        private IFileOperations? _fileOperations;

        [ClassInitialize]
        public static void ClassInit()
        {
            var services = new ServiceCollection(); // Assuming HtmlParser is your custom class implementing IHtmlParser
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            services.AddScoped<IFileOperations, FileOperations>();

            _serviceProvider = services.BuildServiceProvider();
        }
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "FileOperationsTests");

        [TestInitialize]
        public void Initialize()
        {
            if (!Directory.Exists(_testDirectory))
            {
                Directory.CreateDirectory(_testDirectory);
            }
            Assert.IsNotNull(_serviceProvider);
            _fileOperations = _serviceProvider.GetRequiredService<IFileOperations>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public async Task SaveToFile_CreatesAndWritesToFile_WhenProvidedWithContent()
        {
            // Arrange
            string expectedContent = "Test SaveToFile Content";
            string fileName = "TestSaveToFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);

            // Act
            Assert.IsNotNull(_fileOperations);
            await _fileOperations.SaveToFile(expectedContent, filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath), "File should exist after SaveToFile operation.");
            string actualContent = await File.ReadAllTextAsync(filePath);
            Assert.AreEqual(expectedContent, actualContent, "File content should match expected content.");
        }

        [TestMethod]
        public async Task LoadFromFile_ReadsContentCorrectly_WhenFileExists()
        {
            // Arrange
            string expectedContent = "Test LoadFromFile Content";
            string fileName = "TestLoadFromFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);
            await File.WriteAllTextAsync(filePath, expectedContent); // Directly using File IO for setup here

            // Act
            Assert.IsNotNull(_fileOperations);
            string actualContent = await _fileOperations.LoadFromFile(filePath);

            // Assert
            Assert.AreEqual(expectedContent, actualContent, "Content read should match the content written.");
        }

        [TestMethod]
        public async Task LoadFromFile_ReturnsEmptyString_WhenFileDoesNotExist()
        {
            // Arrange
            string fileName = "NonExistentFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);

            // Act
            Assert.IsNotNull(_fileOperations);
            string content = await _fileOperations.LoadFromFile(filePath);

            // Assert
            Assert.AreEqual(string.Empty, content, "Content should be an empty string for non-existent files.");
        }
    }
}
