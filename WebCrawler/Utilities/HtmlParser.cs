using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WebCrawler.DataStructures;

namespace WebCrawler.Utilities
{
    /// <summary>
    /// This class deals with functions that parse HTML.
    /// </summary>
    public class HtmlParser : IHtmlParser
    {
        /// <summary>
        /// This function converts HTML into a tree of IHtmlNode.
        /// </summary>
        /// <param name="html">A string of HTML.</param>
        /// <returns>A tree of IHtmlNode. </returns>
        public IHtmlNode ParseHtmlStringIntoTree(string html)
        {
            var rootNode = new HtmlNode("root");
            var currentParent = rootNode;
            var stack = new Stack<HtmlNode>();

            MatchCollection matches = RegexUtilities.CreateHtmlTagRegex().Matches(html);

            foreach (Match match in matches.Cast<Match>())
            {
                // Skip DOCTYPE and comments
                if (match.Value.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) 
                    || match.Value.StartsWith("<!--", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip this iteration if it's a DOCTYPE declaration or comment
                }

                if (match.Value.StartsWith('<') && !match.Value.StartsWith("</"))
                {
                    // Opening tag or self-closing tag
                    string tagWithAttributes = match.Value.Trim('<', '>');
                    string tagName = tagWithAttributes.Split([' '], 2)[0];
                    var newNode = new HtmlNode(tagName);

                    // Extract attributes
                    var attributesString = tagWithAttributes[tagName.Length..].Trim();
                    MatchCollection attributeMatches = RegexUtilities.HTMLAttributeMatcher().Matches(attributesString);

                    foreach (Match attrMatch in attributeMatches.Cast<Match>())
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
