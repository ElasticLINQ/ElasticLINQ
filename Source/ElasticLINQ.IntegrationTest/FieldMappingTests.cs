// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.IntegrationTest.Models;
using ElasticLinq.Mapping;
using System;
using System.Linq;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class FieldMappingTests
    {
        static readonly ElasticConnectionOptions options = new ElasticConnectionOptions { SearchSizeDefault = 1000 };
        static readonly ElasticConnection connection = new ElasticConnection(Data.Endpoint, index: "integrationtest", options: options);
        static readonly ElasticContext context = new ElasticContext(connection, new TrivialElasticMapping(), retryPolicy: new NoRetryPolicy());

        [Fact]
        public void ToList_Materializes_Complete_Objects()
        {
            var results = context.Query<WebUser>().ToList();

            Assert.Contains(results, w => w.Email == "mlewis1@sitemeter.com");
            Assert.Contains(results, w => w.Email == "cwilson0@mayoclinic.com");
            Assert.Equal(100, results.Count);
        }

        [Fact]
        public void Select_Specific_Fields_Materializes_Correctly()
        {
            var results = context.Query<WebUser>().Select(w => Tuple.Create(w.Email, w.Joined)).ToList();

            Assert.Contains(results, w => w.Item1 == "mlewis1@sitemeter.com");
            Assert.Contains(results, w => w.Item1 == "cwilson0@mayoclinic.com");
            Assert.Equal(100, results.Count);
        }

        [Fact]
        public void Where_Selects_By_Correct_Field_Name()
        {
            var results = context.Query<WebUser>().Where(w => w.Id == 2).ToList();

            Assert.Single(results, w => w.Email == "mlewis1@sitemeter.com");
            Assert.Equal(1, results.Count);
        }
    }
}