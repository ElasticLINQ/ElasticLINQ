using ElasticLinq;
using ElasticLinq.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleApp.Models;
using TestConsoleApp.Utility;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            TestTier3Queries();

            Console.Write("\n\nComplete.");
            Console.ReadKey();
        }

        private static void TestTier3Queries()
        {
            var connection = new ElasticConnection(new Uri("http://192.168.2.12:9200?pretty=true"), TimeSpan.FromSeconds(10), "tier3");
            var provider = new ElasticQueryProvider(connection, new CouchbaseElasticMapping()) { Log = Console.Out };

            var query1 = new ElasticQuery<AccountSubscription>(provider)
//                .Where(s => s.AccountAlias == "t3n")
                .Where(s => s.EndDate.HasValue)
                //.OrderByDescending(s => s.CreateDate)
                //.OrderByDescending(s => ElasticFields.Score)
 //               .OrderBy(s => ElasticFields.Score)
                .Select(s => new { s.Name, ElasticFields.Score });
                ;

            Dump.Query(query1);
        }

        private static void TestBasicElasticProvider()
        {
            var connection = new ElasticConnection(new Uri("http://192.168.2.12:9200"), TimeSpan.FromSeconds(10));
            var provider = new ElasticQueryProvider(connection, new TrivialElasticMapping()) { Log = Console.Out };

            var y = new List<int> { 1962, 1972, 1972, 1980 };

            var i = 7;
            var query = new ElasticQuery<Movie>(provider)
                //.Where(m => !m.Awesome)
                //.Where(m => (m.Year == 1962 && m.Director == "Robert Mulligan") || m.Year < DateTime.Now.Year)
                //.Where(m => y.Contains(m.Year) || int.Equals(m.Year, 1962) || m.Year != 1961)
                .Where(m => m.Year >= 1960 && m.Year <= 1980)
                //.Skip(1)
                //.Take(i + 1)
                //.OrderByDescending(o => o.Year)
                //.OrderByScore()
                .Select(a => Tuple.Create(a.Title, a.Title, a.Year))
                ;

            Dump.Query(query);
        }
    }
}