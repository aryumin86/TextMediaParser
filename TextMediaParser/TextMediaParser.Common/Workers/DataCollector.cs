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
    public class DataCollector : IDataCollector
    {
        public string CollectAuthor(HtmlDocument doc, IEnumerable<AuthorRule> authorRules)
        {
            throw new NotImplementedException();
        }

        public string CollectBody(HtmlDocument doc, IEnumerable<BodyRule> bodyRules)
        {
            throw new NotImplementedException();
        }

        public string CollectCategory(HtmlDocument doc, IEnumerable<CategoryRule> categoryRules)
        {
            throw new NotImplementedException();
        }

        public string CollectDate(HtmlDocument doc, IEnumerable<DateRule> dateRules)
        {
            throw new NotImplementedException();
        }
    }
}
