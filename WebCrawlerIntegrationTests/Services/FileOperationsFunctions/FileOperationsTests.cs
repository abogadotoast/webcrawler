using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.Services.FileOperationsFunctions
{
    [TestClass]
    public class FileOperationsTests
    {
        private static IServiceProvider? _serviceProvider;

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
            // Set up a test directory
            if (!Directory.Exists(_testDirectory))
            {
                Directory.CreateDirectory(_testDirectory);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public async Task LoadFromFile_ReturnsContent_WhenFileExists()
        {
            // Arrange
            string expectedContent = "Hello, World!";
            string filePath = Path.Combine(_testDirectory, "testfile.txt");
            await File.WriteAllTextAsync(filePath, expectedContent);
            var fileOperations = new FileOperations();

            // Act
            string actualContent = await fileOperations.LoadFromFile(filePath);

            // Assert
            Assert.AreEqual(expectedContent, actualContent);
        }

        [TestMethod]
        public async Task LoadFromFile_ReturnsEmptyString_WhenFileDoesNotExist()
        {
            // Arrange
            string filePath = Path.Combine(_testDirectory, "nonexistentfile.txt");
            var fileOperations = new FileOperations();

            // Act
            string actualContent = await fileOperations.LoadFromFile(filePath);

            // Assert
            Assert.AreEqual(string.Empty, actualContent);
        }

        [TestMethod]
        public async Task SaveToFile_CreatesFile_WithCorrectContent()
        {
            // Arrange
            string expectedContent = "Test Content";
            string filePath = Path.Combine(_testDirectory, "saveTestFile.txt");
            var fileOperations = new FileOperations();

            // Act
            await fileOperations.SaveToFile(expectedContent, filePath);
            string actualContent = await File.ReadAllTextAsync(filePath);

            // Assert
            Assert.AreEqual(expectedContent, actualContent);
        }
    }
}
