using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using WebCrawler.DataStructures;
using WebCrawler.Services; // Use your actual namespace

[TestClass]
public class HtmlTreeSearchTests
{
    private Mock<ILogger<HtmlTreeSearch>> _loggerMock;
    private HtmlTreeSearch _htmlTreeSearch;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<HtmlTreeSearch>>();
        _htmlTreeSearch = new HtmlTreeSearch(_loggerMock.Object);
    }

    [TestMethod]
    public void FindDivsWithDataAsyncContext_ReturnsMatchingNodes()
    {
        // Arrange
        var rootNode = new FakeHtmlNode("root", "", "", 0, new List<IHtmlNode>
        {
            new FakeHtmlNode("a", "", "", 0, new List<IHtmlNode>(), new Dictionary<string, string> {{"href", "testUrl"}}),
            new FakeHtmlNode("a", "", "", 0, new List<IHtmlNode> { new FakeHtmlNode("h3") }, new Dictionary<string, string> {{"href", "testUrl"}})
        });

        // Act
        var result = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, "testUrl");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Attributes["href"].Contains("testUrl"));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void FindDivsWithDataAsyncContext_ThrowsException()
    {
        // Arrange
        var rootNode = new FakeHtmlNode("root", "", "", 0, new List<IHtmlNode>
        {
            new FakeHtmlNode("a", "", "", 0, new List<IHtmlNode>(), new Dictionary<string, string> {{"href", "testUrl"}}),
        });

        _loggerMock.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>())).Callback<Exception, string>((ex, msg) => throw ex);

        // Act & Assert
        _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, "testUrl");
    }

    class FakeHtmlNode : IHtmlNode
    {
        public string TagName { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
        public IList<IHtmlNode> Children { get; set; }
        public int RunningIndex { get; set; }

        public FakeHtmlNode(string tagName = "", string content = "", string path = "", int runningIndex = 0, IList<IHtmlNode> children = null, IDictionary<string, string> attributes = null)
        {
            TagName = tagName;
            Content = content;
            Path = path;
            RunningIndex = runningIndex;
            Children = children ?? new List<IHtmlNode>();
            Attributes = attributes ?? new Dictionary<string, string>();
        }

        public void AddChild(IHtmlNode child)
        {
            if (child != null)
            {
                Children.Add(child);
            }
        }
    }
}
