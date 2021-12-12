﻿using HtmlAgilityPack;
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
            int articlesCount = 500;
            int massMediaId = 628;

            Console.WriteLine("APP started");
            var articles = GetArticlesFromDb(massMediaId, articlesCount);
            Stopwatch sw = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Started body rules identification for {articlesCount} articles");
            sw.Start();
            var bodyRules = identifyRules(articles);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ended body rules identification. Done within {sw.Elapsed.TotalSeconds} seconds");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Started appying rules");
            sw.Restart();
            ApplyRules(articles, bodyRules);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ended applying rules. Done within {sw.Elapsed.TotalSeconds} seconds");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static IEnumerable<BodyRule> identifyRules(IEnumerable<Article> articles)
        {
            var RulesIdentificationSettings = new RulesIdentificationSettings
            {
                BodyTagMinimalTextLength = 3,
                BodyTagMinOccurrenceRate = 0.1,
                BodyTagNonUniqueTextMaxOccurrence = 5
            };
            var HtmlHelper = new HtmlHelper(RulesIdentificationSettings);
            var rulesIdentifier = new RulesIdentifier(RulesIdentificationSettings, HtmlHelper);
            var bodyRules = rulesIdentifier.IdentifyBodyRules(articles);

            return bodyRules;

            
        }

        private static void ApplyRules(IEnumerable<Article> articles, IEnumerable<BodyRule> bodyRules)
        {
            foreach (var a in articles)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(a.Html);
                //Console.WriteLine($"--------- LINK: {a.Url}");
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