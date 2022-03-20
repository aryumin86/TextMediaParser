using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.ParsingRules
{
    public class BodyRule : ParsingRule
    {
        /// <summary>
        /// XPath order number on html page.
        /// </summary>
        public int OrderAtHtmlPage { get; set; }
    }
}
