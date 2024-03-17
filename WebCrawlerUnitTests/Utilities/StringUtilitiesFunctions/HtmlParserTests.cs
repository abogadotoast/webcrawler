using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Utilities;

namespace WebCrawlerUnitTests.Utilities.StringUtilitiesFunctions
{
    [TestClass]
    public class HtmlParserTests
    {
        [TestMethod]
        public void ParseHtmlStringIntoTree_ParsesSimpleHtmlCorrectly()
        {
            // Arrange
            var htmlParser = new HtmlParser();
            var html = "<div><p>Test</p></div>";

            // Act
            var rootNode = htmlParser.ParseHtmlStringIntoTree(html);

            // Assert
            Assert.AreEqual("root", rootNode.TagName);
            Assert.AreEqual(1, rootNode.Children.Count);
            Assert.AreEqual("div", rootNode.Children.First().TagName);
            Assert.AreEqual(1, rootNode.Children.First().Children.Count);
            Assert.AreEqual("p", rootNode.Children.First().Children.First().TagName);
            Assert.AreEqual("Test", rootNode.Children.First().Children.First().Content.Trim());
        }

        [TestMethod]
        public void ParseHtmlStringIntoTree_HandlesSelfClosingTags()
        {
            // Arrange
            var htmlParser = new HtmlParser();
            var html = "<div><img src='image.png'/></div>";

            // Act
            var rootNode = htmlParser.ParseHtmlStringIntoTree(html);

            // Assert
            Assert.AreEqual("div", rootNode.Children.First().TagName);
            Assert.AreEqual(1, rootNode.Children.First().Children.Count);
            var imgNode = rootNode.Children.First().Children.First();
            Assert.AreEqual("img", imgNode.TagName);
            Assert.IsTrue(imgNode.Attributes.ContainsKey("src"));
            Assert.AreEqual("image.png", imgNode.Attributes["src"]);
        }

        [TestMethod]
        public void ParseHtmlStringIntoTree_HandlesAttributesCorrectly()
        {
            // Arrange
            var htmlParser = new HtmlParser();
            var html = "<a href='http://example.com'>Link</a>";

            // Act
            var rootNode = htmlParser.ParseHtmlStringIntoTree(html);

            // Assert
            var aNode = rootNode.Children.First();
            Assert.AreEqual("a", aNode.TagName);
            Assert.IsTrue(aNode.Attributes.ContainsKey("href"));
            Assert.AreEqual("http://example.com", aNode.Attributes["href"]);
            Assert.AreEqual("Link", aNode.Content.Trim());
        }

        [TestMethod]
        public void ParseHtmlStringIntoTree_IgnoresCommentsAndDoctype()
        {
            // Arrange
            var htmlParser = new HtmlParser();
            var html = "<!DOCTYPE html><!-- Comment --><p>Content</p>";

            // Act
            var rootNode = htmlParser.ParseHtmlStringIntoTree(html);

            // Assert
            Assert.AreEqual(1, rootNode.Children.Count);
            Assert.AreEqual("p", rootNode.Children.First().TagName);
            Assert.AreEqual("Content", rootNode.Children.First().Content.Trim());
        }
    }
}
