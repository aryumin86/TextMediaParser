using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextMediaParser.Common.Helpers
{
    public class TextHelper : ITextHelper
    {
        private readonly int _minDateStrLength;
        private readonly int _maxDateStrLength;

        private const string _noYearDateFormat1 = "dd.MM hh:mm";
        private const string _noYearDateFormat2 = "dd.MM hh:mm:ss";
        private const string _noYearDateFormat3 = "dd.MM";

        public TextHelper(int minDateStrLength, int maxDateStrLength)
        {
            _minDateStrLength = minDateStrLength;
            _maxDateStrLength = maxDateStrLength;
        }

        public DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;
            if (dateStr.Length < _minDateStrLength || dateStr.Length > _maxDateStrLength)
                return null;

            if (DateTime.TryParse(dateStr, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out DateTime tempRes))
                return tempRes;

            var fixedStrParseRes = FixStrAndTryParseDate(dateStr);
            if (fixedStrParseRes.Item1 == true)
                return fixedStrParseRes.Item2;

            return null;
        }

        /// <summary>
        /// Fixes string and tries to parse date. 
        /// </summary>
        /// <param name="rawDateString"></param>
        /// <returns>tuple with success mark and parsed date</returns>
        public (bool, DateTime) FixStrAndTryParseDate(string rawDateString)
        {
            DateTime tempRes = DateTime.Now;

            if (string.IsNullOrWhiteSpace(rawDateString))
                return (false, DateTime.Now);

            var yesterday = DateTime.Now.AddDays(-1);

            rawDateString = rawDateString.Replace("|", "");
            rawDateString = rawDateString.Replace("Сегодня в", "",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Сегодня", "",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Вчера в", $"{yesterday.Day}.{yesterday.Month}",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Вчера", $"{yesterday.Day}.{yesterday.Month}",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace(" в ", " ");

            rawDateString = rawDateString.Trim(new char[] { ' ', '\n', '\t' });
            rawDateString = Regex.Replace(rawDateString, "\\s+", " ");

            if (DateTime.TryParseExact(rawDateString, "dd M HH:mm", 
                CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, tempRes);
            }                
            else if (DateTime.TryParse(rawDateString, out tempRes))
                return (true, tempRes);
            if(DateTime.TryParseExact(rawDateString, "dd.MM.yyyy", CultureInfo.InvariantCulture,  
                DateTimeStyles.None, out tempRes))
            {
                return (true, tempRes);
            }
            else if (DateTime.TryParseExact(rawDateString, "dd.MM HH:mm",  CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(DateTime.Now.Year, tempRes.Month, 
                    tempRes.Day, tempRes.Hour, tempRes.Minute, 0));
            }                
            else if(DateTime.TryParseExact(rawDateString, "dd.MM HH:mm:ss", CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(DateTime.Now.Year, tempRes.Month,
                    tempRes.Day, tempRes.Hour, tempRes.Minute, tempRes.Second));
            }
            else if (DateTime.TryParseExact(rawDateString, "dd.MM", CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(DateTime.Now.Year, tempRes.Month,
                    tempRes.Day, tempRes.Hour, tempRes.Minute, tempRes.Second));
            }

            else return (false, tempRes);
        }
    }
}
