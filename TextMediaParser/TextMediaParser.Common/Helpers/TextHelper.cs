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

        public TextHelper(int minDateStrLength, int maxDateStrLength)
        {
            _minDateStrLength = minDateStrLength;
            _maxDateStrLength = maxDateStrLength;
        }

        public DateTime? ParseDate(string dateStr, DateTime htmlCollectionDate)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;
            if (dateStr.Length < _minDateStrLength || dateStr.Length > _maxDateStrLength)
                return null;

            var fixedStrParseRes = FixStrAndTryParseDate(dateStr, htmlCollectionDate);
            if (fixedStrParseRes.Item1 == true)
                return fixedStrParseRes.Item2;

            if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempRes))
                return tempRes;            

            return null;
        }

        /// <summary>
        /// Fixes string and tries to parse date. 
        /// </summary>
        /// <param name="rawDateString"></param>
        /// <returns>tuple with success mark and parsed date</returns>
        public (bool, DateTime) FixStrAndTryParseDate(string rawDateString, DateTime htmlCollectionDate)
        {
            DateTime tempRes = htmlCollectionDate;

            var rusCulture = CultureInfo.GetCultureInfo("ru-RU");
            var rusCultureDateFormats = new[]
            {
                "dd MMMM HH:mm", "dd MMMM HH:mm:ss", 
                "dd MMMM HH:mm:ss", "dd MMM HH:mm:ss",
                "d MMMM HH:mm", "d MMMM HH:mm:ss",
                "d MMMM HH:mm:ss", "d MMM HH:mm:ss",
                "dd MMM HH:mm", "dd MMMM HH:mm"
            };
            var invariantCultureFormats = new[]
            {
                "dd M HH:mm","dd.MM.yyyy"
            };

            if (string.IsNullOrWhiteSpace(rawDateString))
                return (false, htmlCollectionDate);

            var yesterday = htmlCollectionDate.AddDays(-1);

            rawDateString = rawDateString.Replace("|", " ");
            rawDateString = rawDateString.Replace("—", " ");
            rawDateString = rawDateString.Replace(",", " ");
            rawDateString = rawDateString.Replace("г.", " ");
            rawDateString = rawDateString.Replace(" в ", " ");
            rawDateString = rawDateString.Replace(" года ", " ");
            rawDateString = rawDateString.Replace(" год ", " ");
            rawDateString = rawDateString.Replace("Сегодня в", string.Empty,
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Сегодня", string.Empty,
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Вчера в", $"{(yesterday.Day < 10 ? "0" + yesterday.Day : yesterday.Day)}.{(yesterday.Month < 10 ? "0" + yesterday.Month : yesterday.Month)}.{yesterday.Year}",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace("Вчера", $"{(yesterday.Day < 10 ? "0" + yesterday.Day : yesterday.Day)}.{(yesterday.Month < 10 ? "0" + yesterday.Month : yesterday.Month)}.{yesterday.Year}",
                StringComparison.InvariantCultureIgnoreCase);
            rawDateString = rawDateString.Replace(" в ", " ");

            rawDateString = rawDateString.Trim(new char[] { ' ', '\n', '\t' });
            rawDateString = Regex.Replace(rawDateString, "\\s+", " ");

            foreach(var format in rusCultureDateFormats)
            {
                if (DateTime.TryParseExact(rawDateString, format,
                    rusCulture, DateTimeStyles.None, out tempRes))
                {
                    return (true, tempRes);
                }
            }

            if (DateTime.TryParse(rawDateString, 
                CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out tempRes))
            {
                return (true, tempRes);
            }

            foreach (var format in invariantCultureFormats)
            {
                if (DateTime.TryParseExact(rawDateString, format,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out tempRes))
                {
                    return (true, tempRes);
                }
            }

            if (DateTime.TryParseExact(rawDateString, "dd.MM HH:mm",  CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(htmlCollectionDate.Year, tempRes.Month, 
                    tempRes.Day, tempRes.Hour, tempRes.Minute, 0));
            }                
            else if(DateTime.TryParseExact(rawDateString, "dd.MM HH:mm:ss", CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(htmlCollectionDate.Year, tempRes.Month,
                    tempRes.Day, tempRes.Hour, tempRes.Minute, tempRes.Second));
            }
            else if (DateTime.TryParseExact(rawDateString, "dd.MM", CultureInfo.InvariantCulture, 0, out tempRes))
            {
                return (true, new DateTime(htmlCollectionDate.Year, tempRes.Month,
                    tempRes.Day, tempRes.Hour, tempRes.Minute, tempRes.Second));
            }

            else if (DateTime.TryParse(rawDateString, out tempRes))
                return (true, tempRes);

            else return (false, tempRes);
        }


        /// <summary>
        /// Tries get date from string with other words e.g.
        /// 09 мая в 00:30 | Обновлено 09 мая в 00:30 (https://riamo.ru/)
        /// or Опубликовано: 12.04.2021 в 15:20 в рубрике  - (http://t7-inform.ru/)
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public string TryGetDateStringFromDateMixedWithOtherWords(string rawString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to get date from url. E.g.
        /// https://www.rosbalt.ru/russia/2021/05/10/1900763.html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public DateTime? TryGetDateFromUrl(string url)
        {
            throw new NotImplementedException();
        }
    }
}
