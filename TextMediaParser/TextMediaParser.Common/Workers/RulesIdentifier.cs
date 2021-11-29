using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.Helpers;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Workers
{
    /// <summary>
    /// Identifies parsing rules.
    /// </summary>
    public class RulesIdentifier : IRulesIdentifier
    {
        private readonly RulesIdentificationSettings _rulesIdentificationSettings;
        private readonly IHtmlHelper _htmlHelper;

        public RulesIdentifier(RulesIdentificationSettings rulesIdentificationSettings,
            IHtmlHelper htmlHelper)
        {
            _rulesIdentificationSettings = rulesIdentificationSettings;
            _htmlHelper = htmlHelper;
        }

        public IEnumerable<AuthorRule> IdentifyAuthorRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BodyRule> IdentifyBodyRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            var res = new List<BodyRule>();

            // key is xpath
            var xpathsContentsInfos = new Dictionary<string, XPathContentsInfo>();

            foreach (var a in articles)
            {
                // sanitizing html of articles
                a.Body = _htmlHelper.SanitizeHtml(a.Body);

                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(a.Body);
                }
                catch(Exception)
                {
                    continue;
                }

                // getting all text nodes from article html
                var textNodes = new List<HtmlNode>();
                _htmlHelper.GetDocTextNodes(doc.DocumentNode, textNodes);

                // filtering text nodes
                textNodes = textNodes.Where(n => _htmlHelper.IsTextNode(n)).ToList();                

                foreach(var textNode in textNodes)
                {
                    // getting md5 hash of text at node
                    var innerTextHash = GetMd5Hash(textNode.InnerText);

                    // if there is no xpath for this text node in xpathsContentsInfos - add new info there
                    // otherwise check if hash of text is already at hashset for this xpath
                    // if there is already this text hash there - increment Count property there
                    // If Count more then limit of uniqueness - mark this xpath as not artilce body part xpath

                    if (!xpathsContentsInfos.ContainsKey(textNode.XPath)){
                        xpathsContentsInfos.Add(textNode.XPath, new XPathContentsInfo
                        {
                            XPath = textNode.XPath,
                            UniqueTextContainer = true,
                            InnerTextsHashes = new HashSet<string>(),
                            Count = 0
                        });
                    }

                    // we already know that this text node is not unique
                    if (xpathsContentsInfos[textNode.XPath].UniqueTextContainer == false)
                        continue;

                    if (!xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Contains(innerTextHash))
                    {
                        xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Add(innerTextHash);
                        xpathsContentsInfos[textNode.XPath].Count = 1;
                    }
                    else
                    {
                        xpathsContentsInfos[textNode.XPath].Count++;
                        xpathsContentsInfos[textNode.XPath].UniqueTextContainer =
                            xpathsContentsInfos[textNode.XPath].Count > _rulesIdentificationSettings.BodyTagNonUniqueTextMaxOccurrence;
                    }
                }
            }

            return res;
        }

        public IEnumerable<CategoryRule> IdentifyCategoryRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DateRule> IdentifyDateRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        private string GetMd5Hash(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString();
            }
        }
    }
}
