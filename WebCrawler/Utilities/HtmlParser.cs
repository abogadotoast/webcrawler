using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebCrawler.DataStructures;

namespace WebCrawler.Utilities
{
    public class HtmlParser : IHtmlParser
    {
        public IHtmlNode ParseHtmlStringIntoTree(string html)
        {
            var rootNode = new HtmlNode("root");
            var currentParent = rootNode;
            var stack = new Stack<HtmlNode>();

            var regex = new Regex(@"(</?[^>]+>)|([^<]+)");
            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("<") && !match.Value.StartsWith("</"))
                {
                    // Opening tag or self-closing tag
                    string tagWithAttributes = match.Value.Trim('<', '>');
                    string tagName = tagWithAttributes.Split(new[] { ' ' }, 2)[0];
                    var newNode = new HtmlNode(tagName);

                    // Extract attributes
                    var attributesString = tagWithAttributes.Substring(tagName.Length).Trim();
                    var attributeMatches = Regex.Matches(attributesString, @"(\w+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""']))+.)[""']?");

                    foreach (Match attrMatch in attributeMatches)
                    {
                        if (attrMatch.Groups.Count == 3)
                        {
                            string attrName = attrMatch.Groups[1].Value;
                            string attrValue = attrMatch.Groups[2].Value;
                            newNode.Attributes[attrName] = attrValue;
                        }
                    }

                    currentParent.AddChild(newNode);

                    if (!match.Value.EndsWith("/>")) // Not a self-closing tag
                    {
                        stack.Push(currentParent);
                        currentParent = newNode;
                    }
                }
                else if (match.Value.StartsWith("</"))
                {
                    // Closing tag
                    if (stack.Count > 0) currentParent = stack.Pop();
                }
                else
                {
                    // Text content
                    currentParent.Content += match.Value;
                }
            }

            return rootNode;
        }
    }
}
