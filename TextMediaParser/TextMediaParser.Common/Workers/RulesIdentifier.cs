using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Workers
{
    /// <summary>
    /// Identifies parsing rules
    /// </summary>
    public class RulesIdentifier : IRulesIdentifier
    {
        private RulesIdentificationSettings _rulesIdentificationSettings;

        public RulesIdentifier(RulesIdentificationSettings rulesIdentificationSettings)
        {
            _rulesIdentificationSettings = rulesIdentificationSettings;
        }

        public IEnumerable<AuthorRule> IdentifyAuthorRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BodyRule> IdentifyBodyRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CategoryRule> IdentifyCategoryRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DateRule> IdentifyDateRules(MassMedia massMedia, IEnumerable<Article> articles)
        {
            throw new NotImplementedException();
        }
    }
}
