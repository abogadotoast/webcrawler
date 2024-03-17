using Microsoft.Extensions.Logging;
using Moq;
using WebCrawler.DataStructures;

namespace WebCrawlerUnitTests.DataStructuresTests
{
    [TestClass]
    public class HtmlTreeSearchTests
    {
        private Mock<ILogger<HtmlTreeSearch>>? _loggerMock;
        private HtmlTreeSearch? _htmlTreeSearch;

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

            Assert.IsNotNull(_htmlTreeSearch);
            // Act
            var result = _htmlTreeSearch.FindDivsWithDataAsyncContext(rootNode, "testUrl");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Attributes["href"].Contains("testUrl"));
        }

        class FakeHtmlNode : IHtmlNode
        {
            public string TagName { get; set; }
            public string Content { get; set; }
            public string Path { get; set; }
            public IDictionary<string, string> Attributes { get; set; }
            public IList<IHtmlNode> Children { get; set; }
            public int RunningIndex { get; set; }

            public FakeHtmlNode(string tagName = "", string content = "", string path = "", int runningIndex = 0, IList<IHtmlNode>? children = null, IDictionary<string, string>? attributes = null)
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
}
