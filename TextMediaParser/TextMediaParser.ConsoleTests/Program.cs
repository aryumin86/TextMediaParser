using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int articlesCount = 200;
            int massMediaId = 36;

            Console.WriteLine("APP started");
            var articles = GetArticlesFromDb(massMediaId, articlesCount);
            Stopwatch sw = new Stopwatch();

            var rulesIdentificationSettings = new RulesIdentificationSettings
            {
                BodyTagMinimalTextLength = 3,
                BodyTagMinOccurrenceRate = 0.1,
                BodyTagNonUniqueTextMaxOccurrence = 5,
                MinDateStrLength = 5,
                MaxDateStrLength = 40,
                DateTagMinOccurrenceRate = 0.3,
                DateTagNonUniqueTextMaxOccurrence = 30
            };

            var textHelper = new TextHelper(
                rulesIdentificationSettings.MinDateStrLength, rulesIdentificationSettings.MaxDateStrLength);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Started body rules identification for {articles.Count()} articles");
            sw.Start();
            var bodyRules = identifyBodyRules(articles, rulesIdentificationSettings, textHelper);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ended body rules identification. Done within {sw.Elapsed.TotalSeconds} seconds");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Started date rules identification for {articles.Count()} articles");
            sw.Restart();
            var dateRules = identifyDateRules(articles, rulesIdentificationSettings, textHelper);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ended date rules identification. Done within {sw.Elapsed.TotalSeconds} seconds");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Started appying rules");
            sw.Restart();

            //exclude date rules from body rules
            bodyRules = bodyRules.Where(bodyRule => !dateRules.Any(dateRule => bodyRule.XPath == dateRule.XPath));

            ApplyRules(articles, bodyRules, dateRules, textHelper);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ended applying rules. Done within {sw.Elapsed.TotalSeconds} seconds");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static IEnumerable<BodyRule> identifyBodyRules(IEnumerable<Article> articles, 
            RulesIdentificationSettings rulesIdentificationSettings, ITextHelper textHelper)
        {            
            var htmlHelper = new HtmlHelper(rulesIdentificationSettings);
            var rulesIdentifier = new RulesIdentifier(
                rulesIdentificationSettings, htmlHelper, textHelper);

            var bodyRules = rulesIdentifier.IdentifyBodyRules(articles);
            return bodyRules;
        }

        private static IEnumerable<DateRule> identifyDateRules(IEnumerable<Article> articles,
            RulesIdentificationSettings rulesIdentificationSettings, ITextHelper textHelper)
        {
            var htmlHelper = new HtmlHelper(rulesIdentificationSettings);
            var rulesIdentifier = new RulesIdentifier(
                rulesIdentificationSettings, htmlHelper, textHelper);

            var dateRules = rulesIdentifier.IdentifyDateRules(articles);

            return dateRules;
        }

        private static void ApplyRules(IEnumerable<Article> articles, IEnumerable<BodyRule> bodyRules, 
            IEnumerable<DateRule> dateRules, ITextHelper textHelper)
        {
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
                            //Console.WriteLine(rule.XPath + ": " + textAtNode + Environment.NewLine + Environment.NewLine);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                foreach (var rule in dateRules.OrderBy(r => r.Priority))
                {
                    try
                    {
                        var textAtNode = doc.DocumentNode.SelectSingleNode(rule.XPath)?.InnerText;
                        if (!string.IsNullOrWhiteSpace(textAtNode) && textHelper.ParseDate(textAtNode).HasValue)
                        {
                            Console.WriteLine(rule.XPath + ": " + textAtNode + Environment.NewLine + Environment.NewLine);
                            break;
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
