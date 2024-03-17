using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.DataStructures;

namespace WebCrawlerUnitTests.DataStructuresTests
{
    [TestClass]
    public class HtmlNodeTests
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var tagName = "div";
            var content = "Example Content";
            var runningIndex = 1;

            // Act
            var node = new HtmlNode(tagName, content, runningIndex);

            // Assert
            Assert.AreEqual(tagName, node.TagName);
            Assert.AreEqual(content, node.Content);
            Assert.AreEqual(runningIndex, node.RunningIndex);
            Assert.IsNotNull(node.Attributes);
            Assert.IsNotNull(node.Children);
        }

        [TestMethod]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var node = new HtmlNode();
            var tagName = "a";
            var content = "Link";
            var runningIndex = 2;

            // Act
            node.TagName = tagName;
            node.Content = content;
            node.RunningIndex = runningIndex;

            // Assert
            Assert.AreEqual(tagName, node.TagName);
            Assert.AreEqual(content, node.Content);
            Assert.AreEqual(runningIndex, node.RunningIndex);
        }

        [TestMethod]
        public void AddChild_AddsChildNodeCorrectly()
        {
            // Arrange
            var parent = new HtmlNode();
            var child = new HtmlNode("span", "Child Node");

            // Act
            parent.AddChild(child);

            // Assert
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(child, parent.Children.First());
        }

        [TestMethod]
        public void Attributes_ManipulationWorksCorrectly()
        {
            // Arrange
            var node = new HtmlNode();
            var attributeName = "href";
            var attributeValue = "http://example.com";

            // Act
            node.Attributes[attributeName] = attributeValue;

            // Assert
            Assert.IsTrue(node.Attributes.ContainsKey(attributeName));
            Assert.AreEqual(attributeValue, node.Attributes[attributeName]);
        }

        [TestMethod]
        public void Children_ManipulationWorksCorrectly()
        {
            // Arrange
            var parent = new HtmlNode();
            var child1 = new HtmlNode("div");
            var child2 = new HtmlNode("span");

            // Act
            parent.AddChild(child1);
            parent.AddChild(child2);

            // Assert
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreSame(child1, parent.Children[0]);
            Assert.AreSame(child2, parent.Children[1]);
        }
    }

}
