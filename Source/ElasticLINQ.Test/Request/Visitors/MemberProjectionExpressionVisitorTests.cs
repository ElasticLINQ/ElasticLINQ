// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class MemberProjectionExpressionVisitorTests
    {
        private class Sample
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }
        private readonly IElasticMapping validMapping = new TrivialElasticMapping();

        [Fact]
        public void Rebind_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind("prefix", null, Expression.Constant(1)));
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind("prefix", validMapping, null));
        }

        [Fact]
        public void RebindCollectsSinglePropertyFieldName()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => s.Name);
            var rebound = MemberProjectionExpressionVisitor.Rebind("prefix", validMapping, source.Expression);

            Assert.Contains("prefix.name", rebound.Collected);
            Assert.Equal(1, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsAnonymousProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => new { s.Name, s.Id, score = ElasticFields.Score });
            var rebound = MemberProjectionExpressionVisitor.Rebind("prefix", validMapping, source.Expression);

            Assert.Contains("prefix.name", rebound.Collected);
            Assert.Contains("prefix.id", rebound.Collected);
            Assert.Contains("_score", rebound.Collected);
            Assert.Equal(3, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsTupleCreateProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => Tuple.Create(s.Name, s.Id, ElasticFields.Score));
            var rebound = MemberProjectionExpressionVisitor.Rebind("prefix", validMapping, source.Expression);

            Assert.Contains("prefix.name", rebound.Collected);
            Assert.Contains("prefix.id", rebound.Collected);
            Assert.Contains("_score", rebound.Collected);
            Assert.Equal(3, rebound.Collected.Count());
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsTokenFromDictionaryIfKeyFound()
        {
            var expected = new Sample { Id = "T-900", Name = "Cameron" };
            const string key = "Summer";
            var dictionary = new Dictionary<string, JToken> { { key, JToken.FromObject(expected) } };

            var actual = (Sample)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, key, typeof(Sample));

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsDefaultObjectIfKeyNotFoundForValueType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (int)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "Any", typeof(int));

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsNullIfKeyNotFoundForReferenceType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (Sample)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "Any", typeof(Sample));

            Assert.Null(actual);
        }
    }
}
