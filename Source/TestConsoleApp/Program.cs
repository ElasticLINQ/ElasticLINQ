﻿// Copyright (c) Tier 3 Inc. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;
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

            var y = new List<int> { 1962, 1972, 1972, 1980 };

            var i = 7;
            var query = new ElasticQuery<Movie>(elasticProvider)
                //.Where(m => (m.Year == "1962" && m.Director == "Robert Mulligan") || (m.Year.Equals("1972")))
                .Where(m => (new [] { 1960, 1963, 1964 }).Contains(m.Year) || int.Equals(m.Year, 1962) || m.Year == 1961)
                //.Skip(1)
                .Take(i + 1)
                .OrderByDescending(o => o.Year)
                .ThenByScore()
                .Select(a => Tuple.Create(a.Title, a.Title, a.Year))
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
        public int Year;

        public override string ToString()
        {
            return string.Format("{0} ({1}), {2}", Title, Year, Director);
        }
    }
}