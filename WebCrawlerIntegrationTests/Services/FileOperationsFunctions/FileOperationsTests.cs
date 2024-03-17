using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerIntegrationTests.Services.FileOperationsFunctions
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
            string filePath = Path.Combine(_testDirectory, fileName);
            await File.WriteAllTextAsync(filePath, expectedContent);

            var content = await _fileOperations.LoadFromFile(filePath);

            Assert.AreEqual(expectedContent, content);
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Never());
            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [TestMethod]
        public async Task LoadFromFile_ReturnsEmpty_WhenFileDoesNotExist()
        {
            string filePath = Path.Combine(_testDirectory, "nonexistent.txt");

            var content = await _fileOperations.LoadFromFile(filePath);

            Assert.AreEqual(string.Empty, content);
            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        [TestMethod]
        public async Task SaveToFile_CreatesFile_WhenItDoesNotExist()
        {
            string expectedContent = "Test content";
            string fileName = "createFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);

            await _fileOperations.SaveToFile(expectedContent, filePath);

            Assert.IsTrue(File.Exists(filePath));
            string actualContent = await File.ReadAllTextAsync(filePath);
            Assert.AreEqual(expectedContent, actualContent);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        [TestMethod]
        public async Task SaveToFile_LogsButDoesNotOverwrite_WhenFileExists()
        {
            string initialContent = "Initial content";
            string fileName = "existingFile.txt";
            string filePath = Path.Combine(_testDirectory, fileName);
            await File.WriteAllTextAsync(filePath, initialContent);

            await _fileOperations.SaveToFile("New content", filePath);

            string actualContent = await File.ReadAllTextAsync(filePath);
            Assert.AreEqual(initialContent, actualContent);
            _mockLogger.VerifyLog(LogLevel.Information, Times.Once(), $"The file {filePath} already exists. No new file was created.");
            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }
    }

    public static class MockExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, Times times, string message = null)
        {
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == logLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => message == null || v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                times);
        }
    }

}
