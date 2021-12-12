using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.Helpers;
using TextMediaParser.Common.ParsingRules;
using TextMediaParser.Common.Workers;

namespace TextMediaParser.ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            identifyAndApplyRules();

            Console.ReadLine();
        }

        private static void identifyAndApplyRules()
        {
            var articles = GetArticlesFromDb(628, 250);

            var RulesIdentificationSettings = new RulesIdentificationSettings
            {
                BodyTagMinimalTextLength = 3,
                BodyTagMinOccurrenceRate = 0.1,
                BodyTagNonUniqueTextMaxOccurrence = 5
            };
            var HtmlHelper = new HtmlHelper(RulesIdentificationSettings); ;

            var rulesIdentifier = new RulesIdentifier(RulesIdentificationSettings, HtmlHelper);

            var bodyRules = rulesIdentifier.IdentifyBodyRules(articles);

            foreach (var a in articles)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(a.Html);
                Console.WriteLine($"--------- LINK: {a.Url}");
                foreach (var rule in bodyRules)
                {
                    try
                    {
                        var textAtNode = doc.DocumentNode.SelectSingleNode(rule.XPath)?.InnerText;
                        if (!string.IsNullOrWhiteSpace(textAtNode))
                        {
                            Console.WriteLine(rule.XPath + ": " + textAtNode + Environment.NewLine + Environment.NewLine);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Get articles with html from files.
        /// </summary>
        /// <param name="massMediaId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<Article> GetArticlesFromDb(int massMediaId, int count)
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
