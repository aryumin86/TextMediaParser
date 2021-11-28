using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.ParsingRules;
using TextMediaParser.Common.Workers;
using Xunit;

namespace TextMediaParser.Common.Tests
{
    public class ParseRulesIdentifiertests
    {
        [Fact]
        public void RulesIdentifier_identifiesArticlesBodyRules()
        {
            var rulesIdentificationSettings = new RulesIdentificationSettings
            {

            };

            var massMedia = new MassMedia
            {
                Id = 1,
                Title = "some mass media",
                Url = "xxx.ru"
            };

            var articles = GetArticlesFromDb(22216, 500);

            var rulesIdentifier = new RulesIdentifier(rulesIdentificationSettings);
            var bodyRules = rulesIdentifier.IdentifyBodyRules(massMedia, articles);
            Assert.NotEmpty(bodyRules);
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
                    Url = (string)reader["Url"]
                };

                res.Add(a);
            }

            return res;
        }
    }
}
