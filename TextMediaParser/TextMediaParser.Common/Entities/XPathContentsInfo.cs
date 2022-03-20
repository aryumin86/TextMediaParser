using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.Entities
{
    /// <summary>
    /// Information about data at container with specific xpath
    /// </summary>
    public class XPathContentsInfo
    {
        public string XPath { get; set; }
        public bool UniqueTextContainer { get; set; }
        public HashSet<string> InnerTextsHashes { get; set; }
        public int Count { get; set; } = 0;
        public int OrderAtHtmlPage { get; set; }
    }
}
