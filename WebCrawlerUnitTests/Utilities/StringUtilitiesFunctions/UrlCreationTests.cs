using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerUnitTests.Utilities.StringUtilitiesFunctions
{
    [TestClass]
    public class UrlCreationTests
    {
        [TestMethod]
        public void CreateLookupURL_ValidInputs_ReturnsCorrectUrl()
        {
            // Arrange
            var keywords = new List<string> { "test", "unit" };
            int numberOfSearchResults = 10;
            var expectedUrl = "https://www.google.com/search?num=10&q=test+unit";

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.AreEqual(expectedUrl, resultUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateLookupURL_NumberOfSearchResultsOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var keywords = new List<string> { "test" };
            int numberOfSearchResults = 101; // Outside valid range

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateLookupURL_NullKeywords_ThrowsArgumentNullException()
        {
            // Act
            var resultUrl = StringUtilities.CreateLookupURL(10, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLookupURL_EmptyKeywordsList_ThrowsArgumentException()
        {
            // Arrange
            var keywords = new List<string>();

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(10, keywords);
        }
        [TestMethod]
        public void CreateLookupURL_MinNumberOfSearchResults_ReturnsCorrectUrl()
        {
            // Arrange
            var keywords = new List<string> { "edge", "case" };
            int numberOfSearchResults = 1; // Minimum valid value
            var expectedUrl = "https://www.google.com/search?num=1&q=edge+case";

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.AreEqual(expectedUrl, resultUrl);
        }

        [TestMethod]
        public void CreateLookupURL_MaxNumberOfSearchResults_ReturnsCorrectUrl()
        {
            // Arrange
            var keywords = new List<string> { "maximum", "value" };
            int numberOfSearchResults = 100; // Maximum valid value
            var expectedUrl = "https://www.google.com/search?num=100&q=maximum+value";

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.AreEqual(expectedUrl, resultUrl);
        }

        [TestMethod]
        public void CreateLookupURL_KeywordsWithSpecialCharacters_ReturnsEncodedUrl()
        {
            // Arrange
            var keywords = new List<string> { "C#", "ASP.NET" };
            int numberOfSearchResults = 10;
            var expectedUrl = "https://www.google.com/search?num=10&q=C%23+ASP.NET"; // Assuming special characters are encoded if necessary

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.AreEqual(expectedUrl, resultUrl);
        }

        [TestMethod]
        public void CreateLookupURL_MixedCaseKeywords_ReturnsCorrectUrl()
        {
            // Arrange
            var keywords = new List<string> { "Mixed", "Case", "Keywords" };
            int numberOfSearchResults = 10;
            var expectedUrl = "https://www.google.com/search?num=10&q=Mixed+Case+Keywords";

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.AreEqual(expectedUrl, resultUrl);
        }

        // This test assumes you adjust CreateLookupURL to URL-encode keywords, 
        // which is necessary for a correct URL but wasn't originally specified in your method.
        [TestMethod]
        public void CreateLookupURL_KeywordsRequireEncoding_ReturnsEncodedUrl()
        {
            // Arrange
            var keywords = new List<string> { "hello world", "test/me" };
            int numberOfSearchResults = 5;
            var expectedUrl = "https://www.google.com/search?num=5&q=hello+world+test%2Fme";

            // Act
            var resultUrl = StringUtilities.CreateLookupURL(numberOfSearchResults, keywords);

            // Assert
            Assert.IsTrue(Uri.IsWellFormedUriString(resultUrl, UriKind.Absolute));
            Assert.AreEqual(expectedUrl, resultUrl);
        }
    }
}
