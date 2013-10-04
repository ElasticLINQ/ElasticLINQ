// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using ElasticLinq;
using ElasticLinq.Mapping;
using System;
using System.Linq;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            TestBasicElasticProvider();

            Console.Write("\n\nComplete.");
            Console.ReadKey();
        }

        private static void TestBasicElasticProvider()
        {
            var connection = new ElasticConnection(new Uri("http://192.168.2.7:9200"), TimeSpan.FromSeconds(10), preferGetRequests: false);
            var elasticProvider = new ElasticQueryProvider(connection, new TrivialElasticMapping()) { Log = Console.Out };

            var i = 7;
            var query = new ElasticQuery<Movie>(elasticProvider)
                .Where(m => (m.Year == "1962" && m.Director == "Robert Mulligan") || (m.Year == "1972"))
                //.Skip(1)
                //.Take(i + 1)
                //.OrderByDescending(o => o.Year)
                //.ThenByScore()
                //.Select(a => new Tuple<string, string, string>(a.Title, a.Title, a.Year));
                ;

            DumpQuery(query);
        }

        private static void DumpQuery<T>(IQueryable<T> query)
        {
            Console.WriteLine(query);

            var results = query.ToList();

            Console.WriteLine("\nResults:");

            foreach (var item in results)
                Console.WriteLine(item);
        }
    }

    public class Movie
    {
        public string Title;
        public string Director;
        public string Year;

        public override string ToString()
        {
            return string.Format("{0} ({1}), {2}", Title, Year, Director);
        }
    }
}