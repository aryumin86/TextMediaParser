using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Workers
{
    public interface IRulesIdentifier
    {
        public IEnumerable<BodyRule> IdentifyBodyRules(IEnumerable<Article> articles);
        public IEnumerable<DateRule> IdentifyDateRules(IEnumerable<Article> articles);
        public IEnumerable<CategoryRule> IdentifyCategoryRules(IEnumerable<Article> articles);
        public IEnumerable<AuthorRule> IdentifyAuthorRules(IEnumerable<Article> articles);
    }
}
