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

        /// <summary>
        /// Tries get date from string with other words e.g.
        /// 09 мая в 00:30 | Обновлено 09 мая в 00:30 (https://riamo.ru/)
        /// or Опубликовано: 12.04.2021 в 15:20 в рубрике  - (http://t7-inform.ru/)
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public string TryGetDateStringFromDateMixedWithOtherWords(string rawString);

        /// <summary>
        /// Tries to get date from url. E.g.
        /// https://www.rosbalt.ru/russia/2021/05/10/1900763.html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public DateTime? TryGetDateFromUrl(string url);
    }
}
