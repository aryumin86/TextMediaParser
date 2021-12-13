using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMediaParser.Common.Helpers;
using TextMediaParser.Common.ParsingRules;
using Xunit;

namespace TextMediaParser.Common.Tests.Helpers
{
    public class CommonTextHelperFixture
    {
        public readonly RulesIdentificationSettings RulesIdentificationSettings;
        public readonly ITextHelper TextHelper;

        public CommonTextHelperFixture()
        {
            RulesIdentificationSettings = new RulesIdentificationSettings
            {
                
            };
            TextHelper = new TextHelper(5, 40);
        }
    }

    public class TextHelperTests : IClassFixture<CommonTextHelperFixture>
    {
        private readonly CommonTextHelperFixture _commonTextHelperFixture;
        public TextHelperTests(CommonTextHelperFixture commonTextHelperFixture)
        {
            _commonTextHelperFixture = commonTextHelperFixture;
        }

        [Trait("Category", "Unit")]
        [Theory]
        [InlineData("2008-05-01T07:34:42-5:00")]
        [InlineData("2008-05-01 7:34:42Z")]
        [InlineData("Thu, 01 May 2008 07:34:42 GMT")]
        public void ParseDate_BasicFormats_ParseSuccess(string dateStr)
        {
            var date = _commonTextHelperFixture.TextHelper.ParseDate(dateStr);
            Assert.True(date.HasValue);
        }

        [Trait("Category", "Unit")]
        [Theory]
        [MemberData(nameof(DateParsingTestDatagenerator.GetDateTestDatas), MemberType = typeof(DateParsingTestDatagenerator))]
        public void ParseDate_MassMediasFormats_ParseSuccess(string dateStr, DateTime expectedDate)
        {
            var parsedDate = _commonTextHelperFixture.TextHelper.ParseDate(dateStr);

            Assert.True(parsedDate.HasValue);
            Assert.True(parsedDate.Value.Year == expectedDate.Year);
            Assert.True(parsedDate.Value.Month == expectedDate.Month);
            Assert.True(parsedDate.Value.Day == expectedDate.Day);
            Assert.True(parsedDate.Value.Hour == expectedDate.Hour);
            Assert.True(parsedDate.Value.Minute == expectedDate.Minute);
        }
    }

    public class DateParsingTestDatagenerator : IEnumerable<object[]>
    {
        public static IEnumerable<object[]> GetDateTestDatas()
        {
            yield return new object[]
            {
                "12 декабря, 00:40",
                new DateTime(2021, 12, 12, 00, 40, 0)
            };
            yield return new object[]
            {
                "10 декабря 2021 г. | 17:46",
                new DateTime(2021, 12, 10, 17, 46, 0)
            };
            yield return new object[]
            {
                "11.12 18:46",
                new DateTime(2021, 12, 11, 18, 46, 0)
            };
            yield return new object[]
            {
                "12 декабря 2021, 16:37",
                new DateTime(2021, 12, 12, 16, 37, 0)
            };
            yield return new object[]
            {
                "11 декабря",
                new DateTime(DateTime.Now.Year, 12, 11, 0, 0, 0)
            };
            yield return new object[]
            {
                "10:43, 12 декабря 2021",
                new DateTime(2021, 12, 12, 10, 43, 0)
            };
            yield return new object[]
            {
                "11.12 14:56",
                new DateTime(DateTime.Now.Year, 12, 11, 14, 56, 0)
            };
            yield return new object[]
            {
                "10 декабря 2021 г. 14:12:13",
                new DateTime(2021, 12, 10, 14, 12, 13)
            };
            yield return new object[]
            {
                "10 декабря в 22:10",
                new DateTime(DateTime.Now.Year, 12, 10, 22, 10, 0)
            };
            yield return new object[]
            {
                "Сегодня в 18:57",
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 57, 0)
            };
            yield return new object[]
            {
                "Вчера в 21:57",
                new DateTime(DateTime.Now.AddDays(-1).Year, DateTime.Now.AddDays(-1).Month,
                DateTime.Now.AddDays(-1).Day, 21, 57, 0)
            };
            yield return new object[]
            {
                "09.12.2021",
                new DateTime(2021, 12, 9, 0, 0, 0)
            };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
