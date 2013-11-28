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

            {
                // QueryString simple case
                var query = context.Query<AccountSubscription>()
                    .QueryString("buck or bucket")
                    .Where(s => s.EndDate.HasValue)
                    .Select(a => new { a.Name, ElasticFields.Score });
                Dump.Query(query);
            }

            {
                // Query based searching
                var query = context.Query<AccountSubscription>()
                    .Query(s => s.AccountAlias == "term")
                    .Select(a => new { a.Name, ElasticFields.Score });
                Dump.Query(query);
            }

            {
                // Filter AND between a TERM and an EXISTS
                var query = context.Query<AccountSubscription>()
                    .Where(s => s.AccountAlias == "t3n" && s.EndDate.HasValue)
                    .OrderBy(s => s.EndDate)
                    .ThenByDescending(s => ElasticFields.Score);
                Dump.Query(query);
            }

            {
                // Query TERM but then filter EXISTS - one possibility for the query/filter distinction
                var query = context.Query<AccountSubscription>()
                    .Query(s => s.AccountAlias == "t3n")
                    .Where(s => s.EndDate.HasValue)
                    .Take(5);
                Dump.Query(query);
            }

            {
                // Projecting into anonymous objects and FIELDS selection
                var query = context.Query<AccountSubscription>()
                    .Select(a => new { a.SubscriptionId, a.Name });
                Dump.Query(query);
            }

            {
                // Projecting into tuples with whole entity reference and score with IN
                var aliases = new[] { "t3n", "dpg", "pbt" };
                var query = context.Query<AccountSubscription>()
                    .Where(a => aliases.Contains(a.AccountAlias))
                    .Select(a => Tuple.Create(a, ElasticFields.Score));
                Dump.Query(query);
            }

            Console.Write("\n\nComplete.");
            Console.ReadKey();
        }
    }
}