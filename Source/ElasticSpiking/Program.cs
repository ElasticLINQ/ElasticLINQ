using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IQToolkit.Data.Common;
using IQToolkit.Data.ElasticSearch;
using IQToolkit.Data.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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

    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    class ElasticResponse
    {
        public int took;
        public bool timed_out;
        public ShardStats _shards;
        public Hits hits;
    }

    [DebuggerDisplay("{hits.Count} hits of {total}")]
    class Hits
    {
        public long total;
        public double max_score;
        public List<Hit> hits;
    }

    [DebuggerDisplay("{_type} in {_index} id {_id}")]
    class Hit
    {
        public string _index;
        public string _type;
        public string _id;
        public double _score;
        public Source _source;
    }

    [DebuggerDisplay("{meta.id}")]
    class Source
    {
        public JObject doc;
        public Meta meta;
    }

    [DebuggerDisplay("{id} rev {rev}")]
    class Meta
    {
        public string id;
        public string rev;
        public int? expiration;
        public int? flags;
    }

    [DebuggerDisplay("{failed} failed, {successful} success")]
    class ShardStats
    {
        public int total;
        public int successful;
        public int failed;
    }
}