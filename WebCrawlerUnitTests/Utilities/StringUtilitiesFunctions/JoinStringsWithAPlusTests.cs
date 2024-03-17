using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerUnitTests.Utilities.StringUtilitiesFunctions
{
    [TestClass]
    public class JoinStringsWithAPlusTests
    {
        [TestMethod]
        public void JoinStringsWithAPlus_WithNormalStrings_ReturnsCorrectlyJoinedString()
        {
            // Arrange
            var keywords = new List<string> { "hello", "world" };
            var expected = "hello+world";

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithUrlUnsafeStrings_ReturnsCorrectlyEncodedAndJoinedString()
        {
            // Arrange
            var keywords = new List<string> { "hello world", "test/me" };
            var expected = "hello+world+test%2fme"; // Note how spaces and slashes are encoded

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithEmptyStrings_ReturnsCorrectlyJoinedString()
        {
            // Arrange
            var keywords = new List<string> { "", "" };
            var expected = "+";

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithNullKeywords_ReturnsStringEmpty()
        {
            // Act & Assert
            var result = StringUtilities.JoinStringsWithAPlus(null);
            Assert.AreEqual(result, string.Empty);
        }
    }
}
