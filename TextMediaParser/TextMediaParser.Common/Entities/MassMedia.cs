using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.ParsingRules;

namespace TextMediaParser.Common.Entities
{
    public class MassMedia 
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
