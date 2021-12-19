using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.ParsingRules
{
    public class DateRule : ParsingRule
    {
        /// <summary>
        /// Priority of rule using. 0 is the highest, 1 is lower, etc.
        /// </summary>
        public int Priority { get; set; } = 0;
    }
}
