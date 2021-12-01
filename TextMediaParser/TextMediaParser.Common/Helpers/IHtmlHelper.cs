using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Helpers
{
    public interface IHtmlHelper
    {
        /// <summary>
        /// Does node contain only text.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsTextNode(HtmlNode node);

        /// <summary>
        /// Tries to sort html nodes in order that will be the same 
        /// as seen by page visitor.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IEnumerable<HtmlNode> SortNodesAsAtPage(IEnumerable<HtmlNode> nodes);

        /// <summary>
        /// Sanitizes html.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string SanitizeHtml(string html);

        /// <summary>
        /// Cleans text of text node.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rulesIdentificationSettings"></param>
        /// <returns></returns>
        public string CleanTextNode(string text);

        /// <summary>
        /// Is is text node according to its InnerText?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rulesIdentificationSettings"></param>
        /// <returns></returns>
        public bool IsTextNode(string text);

        /// <summary>
        /// Get all text nodes of html document.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="result"></param>
        public void GetDocTextNodes(HtmlNode node, IList<HtmlNode> result);
    }
}
