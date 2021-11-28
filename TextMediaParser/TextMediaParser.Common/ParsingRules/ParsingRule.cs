using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.ParsingRules
{
    public abstract class ParsingRule
    {
        public HashSet<string> XPaths { get; set; }
    }
}
