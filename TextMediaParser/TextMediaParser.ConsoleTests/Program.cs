using HtmlAgilityPack;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TextMediaParser.Common.Entities;
using TextMediaParser.Common.Helpers;
using TextMediaParser.Common.ParsingRules;
using TextMediaParser.Common.Workers;

namespace TextMediaParser.ConsoleTests
{
    class Program
    {
        static string resDir = string.Empty;
        static string logFullName = string.Empty;
        static void Main(string[] args)
        {
            int articlesCount = 200;
            //int massMediaId = 148;

            resDir = args.FirstOrDefault()?.TrimEnd('\\');
            if (!string.IsNullOrWhiteSpace(resDir))
            {
                resDir += "\\";
                var dir = new DirectoryInfo(resDir);
                foreach(var f in dir.GetFiles().Where(x => !x.Name.Contains("log.txt")))
                {
                    f.Delete();
                }

                logFullName = $"{resDir}_{DateTime.Now.ToString("yyyyMMdd_HHmmss_")}log.txt";
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("APP started");

            var massMedias = GetMassMediasWithEnoughHtmlForTesting(articlesCount);
            LogInfo($"{massMedias.Count()} mass medias will be processed",
                ConsoleColor.White, logFullName);
            foreach (var massMedia in massMedias.OrderBy(m => m.Id))
            {
                LogInfo($"Started work with mass media {massMedia.Title} ({massMedia.Id})",
                    ConsoleColor.Green, logFullName);

                var articles = GetArticlesFromDb(massMedia.Id, articlesCount);
                Stopwatch sw = new Stopwatch();

                var rulesIdentificationSettings = new RulesIdentificationSettings
                {
                    BodyTagMinimalTextLength = 3,
                    BodyTagMinOccurrenceRate = 0.05,
                    BodyTagNonUniqueTextMaxOccurrence = 5,
                    MinDateStrLength = 5,
                    MaxDateStrLength = 40,
                    DateTagMinOccurrenceRate = 0.3,
                    DateTagNonUniqueTextMaxOccurrence = 30
                };

                var textHelper = new TextHelper(
                    rulesIdentificationSettings.MinDateStrLength, rulesIdentificationSettings.MaxDateStrLength);

                LogInfo($"Started body rules identification for {massMedia.Title} ({massMedia.Id}) for {articles.Count()} articles",
                    ConsoleColor.Blue, logFullName);
                sw.Start();
                var bodyRules = identifyBodyRules(articles, rulesIdentificationSettings, textHelper);
                LogInfo($"Ended body rules identification for {massMedia.Title} ({massMedia.Id}). Done within {sw.Elapsed.TotalSeconds} seconds. Found {bodyRules.Count()} body rules",
                    ConsoleColor.Green, logFullName);

                LogInfo($"Started date rules identification for {massMedia.Title} ({massMedia.Id}) for {articles.Count()} articles.",
                    ConsoleColor.Blue, logFullName);

                sw.Restart();
                var dateRules = identifyDateRules(articles, rulesIdentificationSettings, textHelper);
                LogInfo($"Ended date rules identification for {massMedia.Title} ({massMedia.Id}). Done within {sw.Elapsed.TotalSeconds} seconds. Found {dateRules.Count()} date rules",
                    ConsoleColor.Green, logFullName);

                LogInfo($"Started appying rules",
                    ConsoleColor.Blue, logFullName);

                sw.Restart();

                //exclude date rules from body rules
                bodyRules = bodyRules.Where(bodyRule => !dateRules.Any(dateRule => bodyRule.XPath == dateRule.XPath));

                ApplyRules(articles, bodyRules, dateRules, textHelper, massMedia);
                LogInfo($"Ended applying rules for {massMedia.Title} ({massMedia.Id}). Done within {sw.Elapsed.TotalSeconds} seconds",
                    ConsoleColor.Green, logFullName);

                if (!bodyRules.Any())
                {
                    LogInfo($"!!! No body rules found for massMedia {massMedia.Title} ({massMedia.Id})",
                        ConsoleColor.Red, logFullName);
                }
                if (!dateRules.Any())
                {
                    LogInfo($"!!! No date rules found for massMedia {massMedia.Title} ({massMedia.Id})",
                        ConsoleColor.Red, logFullName);
                }
            }
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void LogInfo(string text, ConsoleColor foregroundColor, string fileFull)
        {
            var time = DateTime.Now;
            string log = $"{time}: {text}";
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(log);
            if (!string.IsNullOrEmpty(fileFull)){
                File.AppendAllText(fileFull, Environment.NewLine + log, Encoding.UTF8);
            }
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
            IEnumerable<DateRule> dateRules, ITextHelper textHelper, MassMedia massMedia)
        {
            var sb = new StringBuilder();
            bool anyOfBodyRulesWasUsed = false;
            bool anyOfDateRulesWasUsed = false;

            foreach (var a in articles)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(a.Html);
                //Console.WriteLine($"--------- LINK: {a.Url}");
                sb.AppendLine($"--------- LINK: {a.Url}");
                foreach (var rule in bodyRules)
                {
                    try
                    {
                        var textAtNode = doc.DocumentNode.SelectSingleNode(rule.XPath)?.InnerText;
                        if (!string.IsNullOrWhiteSpace(textAtNode))
                        {
                            //Console.WriteLine(rule.XPath + ": " + textAtNode + Environment.NewLine + Environment.NewLine);
                            sb.AppendLine($"{textAtNode} {Environment.NewLine}");
                            anyOfBodyRulesWasUsed = true;
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
                        if (!string.IsNullOrWhiteSpace(textAtNode) && textHelper.ParseDate(textAtNode, a.HtmlCollectionDate).HasValue)
                        {
                            var date = textHelper.ParseDate(textAtNode, a.HtmlCollectionDate);
                            //Console.WriteLine(rule.XPath + ": " + textAtNode + Environment.NewLine + Environment.NewLine);
                            sb.AppendLine($"{date} {Environment.NewLine}{Environment.NewLine}");
                            anyOfDateRulesWasUsed = true;
                            break;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }                
            }

            if (!anyOfBodyRulesWasUsed)
                LogInfo($"!!! No found body rule was successfully applied to articles of {massMedia.Title} ({massMedia.Id})", ConsoleColor.Red, logFullName);
            if (!anyOfDateRulesWasUsed)
                LogInfo($"!!! No found date rule was successfully applied to articles of {massMedia.Title} ({massMedia.Id})", ConsoleColor.Red, logFullName);

            if (!string.IsNullOrWhiteSpace(resDir))
                File.WriteAllText(@$"{resDir}\{massMedia.Id}.txt", sb.ToString(), Encoding.UTF8);
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
                    Url = (string)reader["Url"],
                    HtmlCollectionDate = DateTime.Now
                };

                res.Add(a);
            }

            return res;
        }

        private static IEnumerable<MassMedia> GetMassMediasWithEnoughHtmlForTesting(int minArticlesCount)
        {
            List<MassMedia> massMediasWithEnoughHtmlData = new List<MassMedia>();
            var query = @"
select a.""MassMediaId"", count(*), max(m.""Title"") as Title
from public.""Articles"" as a
join public.""MassMedias"" as m on m.""Id"" = a.""MassMediaId""
where a.""Html"" is not null
group by a.""MassMediaId""
having count(*) >= @minArticlesCount
order by count(*) desc;
";

            using var conn = new NpgsqlConnection("Host=127.0.0.1;Database=MassMediaRunnerDb;Uid=postgres;Password=postgres;Port=5432;");
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@minArticlesCount", minArticlesCount);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var mm = new MassMedia
                {
                    Id = (int)reader["MassMediaId"],
                    Title = (string)reader["Title"]
                };

                massMediasWithEnoughHtmlData.Add(mm);
            }

            return massMediasWithEnoughHtmlData;
        }
    }
}
