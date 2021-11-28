using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// Article main content.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Raw page html from mass media website.
        /// </summary>
        public string Html { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }

        /// <summary>
        /// Date of article publication.
        /// </summary>
        public DateTime? PubDate { get; set; }
    }
}
