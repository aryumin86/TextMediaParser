using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextMediaParser.Common.Helpers
{
    /// <summary>
    /// Works with text data.
    /// </summary>
    public interface ITextHelper
    {
        /// <summary>
        /// Identifies date from string.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public DateTime? ParseDate(string dateStr);
    }
}
