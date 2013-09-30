using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ElasticSpiking.BasicProvider;
using IQToolkit.Data.Common;
using IQToolkit.Data.ElasticSearch;
using IQToolkit.Data.ElasticSearch.Response;
using IQToolkit.Data.Mapping;
using Newtonsoft.Json;
using ElasticQueryProvider = ElasticSpiking.BasicProvider.ElasticQueryProvider;

namespace ElasticSpiking
{
    internal class Program
    {
        private static readonly Uri endpoint = new Uri("http://10.81.84.12:9200/main/_search");

        private static void Main()
        {
            TestBasicElasticProvider();

            Console.ReadKey();
        }

        private static void TestBasicElasticProvider()
        {
            var elasticProvider = new ElasticQueryProvider(endpoint);
            var query = new IQToolkit.Query<object>(elasticProvider);
            PrintQuery(query);
            foreach (var item in query.ToList())
                Console.WriteLine(item);
        }

        private static void TestBasicDbProvider()
        {
            const string connectionString = @"Server=(local);Database=Northwind;Trusted_Connection=True;";

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var db = new Northwind(con);

                var city = "London";
                var query = db.Customers.Where(c => c.City == city);

                PrintQuery(query);
                foreach (var item in query.ToList())
                    Console.WriteLine("Name: {0}", item.ContactName);

                PrintQuery(query.Select(c => new { A = c.ContactName }));
            }
        }

        private static void PrintQuery<T>(IQueryable<T> query)
        {
            Console.WriteLine("Query:\n{0}\n", query);
        }
    }
}