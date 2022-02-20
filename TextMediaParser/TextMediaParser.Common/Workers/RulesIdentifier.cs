using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
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
        private readonly ITextHelper _textHelper;

        public RulesIdentifier(RulesIdentificationSettings rulesIdentificationSettings,
            IHtmlHelper htmlHelper, ITextHelper textHelper)
        {
            _rulesIdentificationSettings = rulesIdentificationSettings;
            _htmlHelper = htmlHelper;
            _textHelper = textHelper;
        }

        public IEnumerable<AuthorRule> IdentifyAuthorRules(IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BodyRule> IdentifyBodyRules(IEnumerable<Article> articles)
        {
            var res = new List<BodyRule>();

            // key is xpath
            var xpathsContentsInfos = new ConcurrentDictionary<string, XPathContentsInfo>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = _rulesIdentificationSettings.RulesIdentificationParallelism
            };

            //foreach (var a in articles)
            Parallel.ForEach(articles, parallelOptions, a =>
            {
                // sanitizing html of articles
                a.Html = _htmlHelper.SanitizeHtml(a.Html);

                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(a.Html);
                }
                catch (Exception)
                {
                    return;
                }

                // getting all text nodes from article html
                var textNodes = new List<HtmlNode>();
                _htmlHelper.GetDocTextNodes(doc.DocumentNode, textNodes);

                foreach (var textNode in textNodes)
                {
                    // getting md5 hash of text at node
                    var innerTextHash = GetMd5Hash(textNode.InnerText);

                    // if there is no xpath for this text node in xpathsContentsInfos - add new info there
                    // otherwise check if hash of text is already at hashset for this xpath
                    // if there is already this text hash there - increment Count property there
                    // If Count more then limit of uniqueness - mark this xpath as not artilce body part xpath


                    while (!xpathsContentsInfos.ContainsKey(textNode.XPath))
                    {
                        var added = xpathsContentsInfos.TryAdd(textNode.XPath, new XPathContentsInfo
                        {
                            XPath = textNode.XPath,
                            UniqueTextContainer = true,
                            InnerTextsHashes = new HashSet<string>(),
                            Count = 1
                        });

                        if (added)
                            break;
                    }

                    // we already know that this text node is not unique
                    if (xpathsContentsInfos[textNode.XPath].UniqueTextContainer == false)
                        continue;

                    if (!xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Contains(innerTextHash))
                    {
                        xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Add(innerTextHash);
                    }
                    else
                    {
                        xpathsContentsInfos[textNode.XPath].Count++;
                        xpathsContentsInfos[textNode.XPath].UniqueTextContainer =
                            xpathsContentsInfos[textNode.XPath].Count < _rulesIdentificationSettings.BodyTagNonUniqueTextMaxOccurrence;
                    }
                }
            });

            // non unique text xpaths
            var nonUniqueXpaths = new HashSet<string>(xpathsContentsInfos
                .Where(xci => !xci.Value.UniqueTextContainer)
                .Select(xci => xci.Key));

            // excepting xpaths that are not unique
            var minimalOccurenceOfXpthAcrossArticles =
                articles.Count() * _rulesIdentificationSettings.BodyTagMinOccurrenceRate;
            var rareXpaths = new HashSet<string>(xpathsContentsInfos
                .Where(xci => xci.Value.InnerTextsHashes.Count()
                < minimalOccurenceOfXpthAcrossArticles)
                .Select(xci => xci.Key));

            foreach (var xci in xpathsContentsInfos.Keys.Where(x => 
                !nonUniqueXpaths.Contains(x) && !rareXpaths.Contains(x)))
            {
                res.Add(new BodyRule
                {
                    XPath = xci
                });
            }

            // TODO get doc with majority of nodes for identified xpaths and sort them.
            // After that sort rules according to node sorting result
            return res;
        }

        public IEnumerable<CategoryRule> IdentifyCategoryRules(IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DateRule> IdentifyDateRules(IEnumerable<Article> articles)
        {
            var res = new List<DateRule>();

            // key is xpath
            var xpathsContentsInfos = new ConcurrentDictionary<string, XPathContentsInfo>();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = _rulesIdentificationSettings.RulesIdentificationParallelism
            };

            Parallel.ForEach(articles, parallelOptions, a =>
            {
                // sanitizing html of articles
                a.Html = _htmlHelper.SanitizeHtml(a.Html);

                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(a.Html);
                }
                catch (Exception)
                {
                    return;
                }

                // getting all text nodes from article html
                var textNodes = new List<HtmlNode>();
                _htmlHelper.GetDocTextNodes(doc.DocumentNode, textNodes);

                foreach (var textNode in textNodes)
                {
                    // getting md5 hash of text at node
                    var innerTextHash = GetMd5Hash(textNode.InnerText);

                    var mayBeDate = _textHelper.ParseDate(textNode.InnerText);
                    if (mayBeDate == null)
                        continue;
                    else
                    {

                    }

                    // if there is no xpath for this text node in xpathsContentsInfos - add new info there
                    // otherwise check if hash of text is already at hashset for this xpath
                    // if there is already this text hash there - increment Count property there
                    // If Count more then limit of uniqueness - mark this xpath as not artilce body part xpath


                    while (!xpathsContentsInfos.ContainsKey(textNode.XPath))
                    {
                        var added = xpathsContentsInfos.TryAdd(textNode.XPath, new XPathContentsInfo
                        {
                            XPath = textNode.XPath,
                            UniqueTextContainer = true,
                            InnerTextsHashes = new HashSet<string>(),
                            Count = 1
                        });

                        if (added)
                            break;
                    }

                    // we already know that this text node is not unique
                    if (xpathsContentsInfos[textNode.XPath].UniqueTextContainer == false)
                        continue;

                    if (!xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Contains(innerTextHash))
                    {
                        xpathsContentsInfos[textNode.XPath].InnerTextsHashes.Add(innerTextHash);
                    }
                    else
                    {
                        xpathsContentsInfos[textNode.XPath].Count++;
                        xpathsContentsInfos[textNode.XPath].UniqueTextContainer =
                            xpathsContentsInfos[textNode.XPath].Count < _rulesIdentificationSettings.DateTagNonUniqueTextMaxOccurrence;
                    }
                }
            });

            // non unique text xpaths
            var nonUniqueXpaths = new HashSet<string>(xpathsContentsInfos
                .Where(xci => !xci.Value.UniqueTextContainer)
                .Select(xci => xci.Key));

            // excepting xpaths that are not unique
            var minimalOccurenceOfXpthAcrossArticles =
                articles.Count() * _rulesIdentificationSettings.DateTagMinOccurrenceRate;
            var rareXpaths = new HashSet<string>(xpathsContentsInfos
                .Where(xci => xci.Value.InnerTextsHashes.Count()
                < minimalOccurenceOfXpthAcrossArticles)
                .Select(xci => xci.Key));

            var notConcurrentXpathsContentsInfos = xpathsContentsInfos
                .OrderByDescending(x => x.Value.Count)
                .ToDictionary(x => x.Key, x => x.Value);

            // the higher xpath occuerence the higher the priority of this rule. 0 is the highest
            int priority = 0; 
            foreach (var xci in notConcurrentXpathsContentsInfos.Keys.Where(x =>
                !nonUniqueXpaths.Contains(x) && !rareXpaths.Contains(x)))
            {
                res.Add(new DateRule
                {
                    XPath = xci,
                    Priority = priority++
                });
            }

            return res;
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
