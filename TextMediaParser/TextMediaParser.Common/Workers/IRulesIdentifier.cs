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
        public IEnumerable<BodyRule> IdentifyBodyRules(MassMedia massMedia, IEnumerable<Article> articles);
        public IEnumerable<DateRule> IdentifyDateRules(MassMedia massMedia, IEnumerable<Article> articles);
        public IEnumerable<CategoryRule> IdentifyCategoryRules(MassMedia massMedia, IEnumerable<Article> articles);
        public IEnumerable<AuthorRule> IdentifyAuthorRules(MassMedia massMedia, IEnumerable<Article> articles);
    }
}
