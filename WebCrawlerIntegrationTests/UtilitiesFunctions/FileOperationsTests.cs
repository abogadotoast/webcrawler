using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.UtilitiesFunctions
{
    [TestClass]
    public class FileOperationsTests
    {
        private static string? _testDirectory;
        private IFileOperations? _fileOperations;
        private Mock<ILogger<FileOperations>>? _mockLogger;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "FileOperationsTests");
        }

        [TestInitialize]
        public void Initialize()
        {
            Assert.IsNotNull(_testDirectory);
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
            Directory.CreateDirectory(_testDirectory);

            _mockLogger = new Mock<ILogger<FileOperations>>();
            _fileOperations = new FileOperations(_mockLogger.Object);
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
        public async Task LoadFromFile_ReturnsContent_WhenFileExists()
        {
            string expectedContent = "Hello, World!";
            string fileName = "testfile.txt";
            Assert.IsNotNull(_testDirectory);
            string filePath = Path.Combine(_testDirectory, fileName);
            await File.WriteAllTextAsync(filePath, expectedContent);

            Assert.IsNotNull(_fileOperations);
            var content = await _fileOperations.LoadFromFile(filePath);

            Assert.AreEqual(expectedContent, content);
        }

        [TestMethod]
        public async Task LoadFromFile_ReturnsEmpty_WhenFileDoesNotExist()
        {
            Assert.IsNotNull(_testDirectory);
            Assert.IsNotNull(_fileOperations);
            string filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            var content = await _fileOperations.LoadFromFile(filePath);

            Assert.AreEqual(string.Empty, content);
        }

        [TestMethod]
        public async Task SaveToFile_CreatesFile_WhenItDoesNotExist()
        {
            Assert.IsNotNull(_testDirectory);
            Assert.IsNotNull(_fileOperations);
            string expectedContent = "Test content";
            string fileName = "createFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);

            await _fileOperations.SaveToFile(expectedContent, filePath);

            Assert.IsTrue(File.Exists(filePath));
            string actualContent = await File.ReadAllTextAsync(filePath);
            Assert.AreEqual(expectedContent, actualContent);
        }

        [TestMethod]
        public async Task SaveToFile_LogsButDoesNotOverwrite_WhenFileExists()
        {
            Assert.IsNotNull(_testDirectory);
            Assert.IsNotNull(_fileOperations);
            string initialContent = "Initial content";
            string fileName = "existingFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);
            await File.WriteAllTextAsync(filePath, initialContent);

            await _fileOperations.SaveToFile("New content", filePath);

            string actualContent = await File.ReadAllTextAsync(filePath);
            Assert.AreEqual(initialContent, actualContent);
        }
    }
}
