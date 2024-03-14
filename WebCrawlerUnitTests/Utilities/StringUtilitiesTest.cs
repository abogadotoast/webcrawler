using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerUnitTests.Utilities
{
    [TestClass]
    public class StringUtilitiesTests
    {
        [TestMethod]
        public void JoinStringsWithAPlus_WithMultipleStrings_ReturnsCorrectResult()
        {
            // Arrange
            var strings = new List<string> { "Hello", "world", "this", "is", "a", "test" };
            var expected = "Hello + world + this + is + a + test";

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(strings);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithSingleString_ReturnsSingleString()
        {
            // Arrange
            var strings = new List<string> { "Hello" };
            var expected = "Hello";

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(strings);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithEmptyList_ReturnsEmptyString()
        {
            // Arrange
            var strings = new List<string>();
            var expected = "";

            // Act
            var result = StringUtilities.JoinStringsWithAPlus(strings);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinStringsWithAPlus_WithNullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<System.ArgumentNullException>(() => StringUtilities.JoinStringsWithAPlus(null));
        }
    }
}
