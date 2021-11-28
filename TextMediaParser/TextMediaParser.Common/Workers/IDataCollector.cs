using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Workers
{
    /// <summary>
    /// Collects data using parsing rules.
    /// </summary>
    public interface IDataCollector
    {
        public string CollectBody(HtmlDocument doc, IEnumerable<BodyRule> bodyRules);
        public string CollectDate(HtmlDocument doc, IEnumerable<DateRule> dateRules);
        public string CollectAuthor(HtmlDocument doc, IEnumerable<AuthorRule> authorRules);
        public string CollectCategory(HtmlDocument doc, IEnumerable<CategoryRule> categoryRules);
    }
}
