using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.Helpers;
using TextMediaParser.Common.ParsingRules;
using TextMediaParser.Common.Workers;
using Xunit;

namespace TextMediaParser.Common.Tests
{
    public class CommonsFixture
    {
        public readonly RulesIdentificationSettings RulesIdentificationSettings;
        public readonly IHtmlHelper HtmlHelper;
        public readonly ITextHelper TextHelper;

        public CommonsFixture()
        {
            RulesIdentificationSettings = new RulesIdentificationSettings
            {
                BodyTagMinimalTextLength = 3,
                BodyTagMinOccurrenceRate = 0.05,
                BodyTagNonUniqueTextMaxOccurrence = 15
            };
            HtmlHelper = new HtmlHelper(RulesIdentificationSettings);
            TextHelper = new TextHelper(5, 25);
        }       
    }

    public class ParseRulesIdentifiertests : IClassFixture<CommonsFixture>
    {
        private readonly CommonsFixture _commonsFixture;
        public ParseRulesIdentifiertests(CommonsFixture commonsFixture)
        {
            _commonsFixture = commonsFixture;
        }

        [Fact]
        [Trait("Category", "Integrational")]
        public void RulesIdentifier_identifiesArticlesBodyRules()
        {
            var articles = GetArticlesFromDb(22216, 500);

            var rulesIdentifier = new RulesIdentifier(
                _commonsFixture.RulesIdentificationSettings, 
                _commonsFixture.HtmlHelper, _commonsFixture.TextHelper);
            var bodyRules = rulesIdentifier.IdentifyBodyRules(articles);
            Assert.NotEmpty(bodyRules);
        }

        [Fact]
        [Trait("Category", "Integrational")]
        public void RulesIdentifier_IdentifyDateRules_GetsDates()
        {
            var massMediaIds = new[] { 22216, 1644 };
            foreach(var massMediaId in massMediaIds)
            {
                var articles = GetArticlesFromDb(massMediaId, 250);
                var rulesIdentifier = new RulesIdentifier(
                _commonsFixture.RulesIdentificationSettings, 
                _commonsFixture.HtmlHelper, _commonsFixture.TextHelper);
                var dateRules = rulesIdentifier.IdentifyDateRules(articles);
                Assert.NotEmpty(dateRules);
            }
        }

        /// <summary>
        /// Get articles with html from files.
        /// </summary>
        /// <param name="massMediaId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<Article> GetArticlesFromDb(int massMediaId, int count)
        {
            var query = @"
select ""Id"", ""Url"", ""Html""
from public.""Articles""
where ""MassMediaId"" = @id and ""Html"" is not null
limit @count;";

            var res = new List<Article>();
            using var conn = new NpgsqlConnection("Host=127.0.0.1;Database=MassMediaRunnerDb;Uid=postgres;Password=postgres;Port=5432;");
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", massMediaId);
            cmd.Parameters.AddWithValue("@count", count);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var a = new Article
                {
                    Id = (int)reader["Id"],
                    Html = (string)reader["Html"],
                    Url = (string)reader["Url"],
                    HtmlCollectionDate = DateTime.Now
                };

                res.Add(a);
            }

            return res;
        }
    }
}
