using IQToolkit.Data.Common;
using IQToolkit.Data.ElasticSearch;
using IQToolkit.Data.ElasticSearch.Response;
using IQToolkit.Data.Mapping;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElasticSpiking
{
    class Program
    {
        private static readonly Uri endpoint = new Uri("http://10.81.84.12:9200/main/_search");

        static void Main()
        {
            TestReading().Wait();

            Console.ReadKey();
        }

        static async Task TestReading()
        {
            var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(endpoint);
            
            var serializer = new JsonSerializer();
            var textReader = new JsonTextReader(new StreamReader(responseStream));
            var elasticResponse = serializer.Deserialize<ElasticResponse>(textReader);
        }

        static void TestConnection()
        {
            var connection = new ElasticConnection(endpoint);
            var provider = new ElasticQueryProvider(connection, new ImplicitMapping(), new QueryPolicy())
            {
                Log = Console.Out
            };

            var query = provider.GetTable<ElasticResponse>();
            var results = query.ToArray();
        }
    }


}