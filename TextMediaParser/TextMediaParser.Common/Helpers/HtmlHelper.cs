using Ganss.XSS;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Helpers
{
    public class HtmlHelper : IHtmlHelper
    {
        private HtmlSanitizer _htmlSanitizer;
        private RulesIdentificationSettings _rulesIdentificationSettings;

        public HtmlHelper(RulesIdentificationSettings rulesIdentificationSettings)
        {
            _htmlSanitizer = new HtmlSanitizer();
            _rulesIdentificationSettings = rulesIdentificationSettings;
        }

        /// <summary>
        /// Cleans text of text node.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rulesIdentificationSettings"></param>
        /// <returns></returns>
        public string CleanTextNode(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all text nodes of html document.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="result"></param>
        public void GetDocTextNodes(HtmlNode currentNode, IList<HtmlNode> result)
        {
            if (IsTextNode(currentNode))
            {
                result.Add(currentNode);
            }
            else
            {
                foreach (var n in currentNode.ChildNodes)
                    GetDocTextNodes(n, result);
            }
        }

        /// <summary>
        /// Node contains only text?
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsTextNode(HtmlNode node)
        {
            if (node.Name == "p" && node.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Length
                >= _rulesIdentificationSettings.BodyTagMinimalTextLength)
                return true;
            if (node.ChildNodes.Count == 1 && node.ChildNodes.First().Name == "#text")
                return true;
            if (node.ChildNodes.Count == 0 
                && node.InnerText.Replace("\n","").Replace("\r","").Replace(" ", "").Length
                >= _rulesIdentificationSettings.BodyTagMinimalTextLength)
                return true;

            return false;
        }

        /// <summary>
        /// Is is text node according to its InnerText?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rulesIdentificationSettings"></param>
        /// <returns></returns>
        public bool IsTextNode(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sanitizes html.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string SanitizeHtml(string html)
        {
            return _htmlSanitizer.Sanitize(html);
        }

        /// <summary>
        /// Tries to sort html nodes in order that will be the same 
        /// as seen by page visitor.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> SortNodesAsAtPage(IEnumerable<HtmlNode> nodes)
        {
            throw new NotImplementedException();
        }
    }
}
