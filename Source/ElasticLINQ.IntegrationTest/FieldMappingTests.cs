using ElasticLinq.IntegrationTest.Models;
using ElasticLinq.Mapping;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ElasticLinq.IntegrationTest
{
    public class TypePrefixedMapper : CouchbaseElasticMapping
    {
        public override string GetDocumentMappingPrefix(Type type)
        {
            return type.Name.ToLower();
        }

        public override string GetDocumentType(Type type)
        {
            return "doc";
        }
    }

    public class FieldMappingTests
    {
        static readonly IElasticMapping mapping = new TypePrefixedMapper();
        static readonly Uri elasticsearchEndpoint = new Uri("http://elasticlinq.cloudapp.net:9200");
        static readonly ElasticConnectionOptions options = new ElasticConnectionOptions { SearchSizeDefault = 1000 };
        static readonly ElasticConnection connection = new ElasticConnection(elasticsearchEndpoint, index: "integrationtest-nested", options: options);
        static readonly ElasticContext context = new ElasticContext(connection, mapping);

        [Fact]
        public void ToList_Materializes_Complete_Objects()
        {
            var results = context.Query<WebUser>().ToList();

            Assert.Contains(results, w => w.Email == "mlewis1@sitemeter.com");
            Assert.Contains(results, w => w.Email == "cwilson0@mayoclinic.com");
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Select_Specific_Fields_Materializes_Correctly()
        {
            var results = context.Query<WebUser>().Select(w => Tuple.Create(w.Email, w.Joined)).ToList();

            Assert.Contains(results, w => w.Item1 == "mlewis1@sitemeter.com");
            Assert.Contains(results, w => w.Item1 == "cwilson0@mayoclinic.com");
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Where_Selects_By_Correct_Field_Name()
        {
            var results = context.Query<WebUser>().Where(w => w.Id == 2).ToList();

            Assert.Single(results, w => w.Email == "mlewis1@sitemeter.com");
            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void GroupByInt_Returns_Correct_Single_Field()
        {
            var results = context.Query<WebUser>().GroupBy(g => 1).Select(s => s.Max(w => w.Joined)).ToList();

            Assert.Equal(1, results.Count);
            Assert.Equal(new DateTime(2014, 10, 5, 17, 22, 54, DateTimeKind.Utc), results[0]);
        }

        [Fact]
        public void GroupByDateTime()
        {
            DataAssert.Same((IQueryable<WebUser> q) => q.GroupBy(w => w.Joined).Select(g => KeyValuePair.Create(g.Key, g.Count())), true);
        }
    }
}