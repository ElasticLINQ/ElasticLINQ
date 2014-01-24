using ElasticLinq;
using ElasticLinq.Mapping;
using System;
using System.Linq;
using TestConsoleApp.Models;
using TestConsoleApp.Utility;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            var connection = new ElasticConnection(new Uri("http://192.168.2.14:9200")) { Index = "tier3" };
            var context = new ElasticContext(connection, new CouchbaseElasticMapping()) { Log = Console.Out };

            AggregateQueries();
            //DocumentQueries(context);
            //BasicSampleQueries(context);

            Console.Write("\n\nComplete.");
            Console.ReadKey();
        }

        private static void AggregateQueries()
        {
            var movieConnection = new ElasticConnection(new Uri("http://192.168.2.14:9200"));
            var movieContext = new ElasticContext(movieConnection, new TrivialElasticMapping()) { Log = Console.Out };

            movieContext
                .Query<Movie>()
                .Where(a => a.Director == "David Lean")
                .GroupBy(a => a.Director)
                .Select(a => new { a.Key, First = a.Min(b => b.Year), TopRated = a.Max(b => b.Rating), Count = a.Count() })
                .WriteToConsole();

            //var z = movieContext
            //    .Query<Movie>()
            //    .Sum(a => a.Rating);
            //Write.ToConsole(z);
        }

        private static void DocumentQueries(ElasticContext context)
        {
            context
                .Query<Activity>()
                .Where(a => a.AccountAlias == "t3n")
                .Where(a => a.CreatedDate >= new DateTime(2013, 09, 01) && a.CreatedDate <= new DateTime(2013, 09, 08))
                .Take(1000000)
                .OrderByDescending(o => o.CreatedDate)
                .WriteToConsole();

            context
                .Query<AccountSubscription>()
                .Where(s => s.SubscriptionId == "abc" && s.AccountAlias == "t3n" && !s.EndDate.HasValue)
                .OrderByDescending(s => s.CreateDate)
                .WriteToConsole();
        }

        private static void BasicSampleQueries(ElasticContext context)
        {
            // QueryString simple case with exists filter
            context
                .Query<AccountSubscription>()
                .QueryString("buck or bucket")
                .Where(s => s.EndDate.HasValue)
                .Select(a => new { a.Name, ElasticFields.Score })
                .WriteToConsole();

            // Query term returning anonymous object with one field and score projection
            context
                .Query<AccountSubscription>()
                .Query(s => s.AccountAlias == "term")
                .Select(a => new { a.Name, ElasticFields.Score })
                .WriteToConsole();

            // Filter with term and exists combined and multiple ordering
            context
                .Query<AccountSubscription>()
                .Where(s => s.AccountAlias == "t3n" && s.EndDate.HasValue)
                .OrderBy(s => s.EndDate)
                .ThenByDescending(s => ElasticFields.Score)
                .WriteToConsole();

            // Query term then exists filter and skip take
            context
                .Query<AccountSubscription>()
                .Query(s => s.AccountAlias == "t3n")
                .Where(s => s.EndDate.HasValue)
                .Skip(1)
                .Take(5)
                .WriteToConsole();

            // Filter terms from an array with Tuple to return original entity + score
            var aliases = new[] { "t3n", "dpg", "pbt" };
            context
                .Query<AccountSubscription>()
                .Where(a => aliases.Contains(a.AccountAlias))
                .Select(a => Tuple.Create(a, ElasticFields.Score))
                .WriteToConsole();
        }
    }
}