namespace ElasticApiTest
{
    using System;
    using ElasticApi.Connections;
    using ElasticApi.Requests;

    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HttpConnection(new Uri("http://10.2.0.70:9201"));

            //  ClusterHealth
            {
                var request = new ClusterHealthRequest { Index = "toto" };

                var response = ElasticApi.Elastic.ClusterHealth(connection, request);
            }

            //  Index
            {
                var request = new IndexRequest { Index = "toto", Type = "tata", Id = "1", Body = new { Int = 2, String = "tutu" } };

                var response = ElasticApi.Elastic.Index(connection, request);
            }

            //  Get
            {
                var request = new GetRequest { Index = "toto", Type = "tata", Id = "1" };

                var response = ElasticApi.Elastic.Get(connection, request);
            }

            //  Delete
            {
                var request = new DeleteRequest { Index = "toto", Type = "tata", Id = "1" };

                var response = ElasticApi.Elastic.Delete(connection, request);
            }
        }
    }
}
