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

            BasicSampleQueries(context);

            Console.Write("\n\nComplete.");
            Console.ReadKey();
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