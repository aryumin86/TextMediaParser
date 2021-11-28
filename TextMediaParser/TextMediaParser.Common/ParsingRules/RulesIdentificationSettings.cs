using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.ParsingRules
{
    /// <summary>
    /// Settings to identify parsing rules.
    /// </summary>
    public class RulesIdentificationSettings
    {
        #region body

        /// <summary>
        /// Minimal rate of body tag with some xpath occerrence.
        /// (count of texts with some xpath containing some text) / (total articles count).
        /// </summary>
        public double BodyTagMinOccurrenceRate { get; set; } = 0.1;

        /// <summary>
        /// This maximimum number of times the same tag with the same text can occure 
        /// in body (in different articles).
        /// </summary>
        public int BodyTagNonUniqueTextMaxOccurrence { get; set; } = 3;

        /// <summary>
        /// Tag should have this minimal length.
        /// </summary>
        public int BodyTagMinimalTextLength { get; set; } = 3;

        #endregion

        #region date

        #endregion

        #region author

        #endregion

        #region category

        #endregion
    }
}
