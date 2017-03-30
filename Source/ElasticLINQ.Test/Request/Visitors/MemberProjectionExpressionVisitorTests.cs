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
        class Sample
        {
            public string Name { get; set; }
            public string Id { get; set; }

            public Sample Nested { get; set; }
        }
        readonly IElasticMapping validMapping = new TrivialElasticMapping();

        [Fact]
        public void Rebind_GuardClauses()
        {
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind(typeof(Sample), null, Expression.Constant(1)));
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind(typeof(Sample), validMapping, null));
        }

        [Fact]
        public void RebindCollectsSinglePropertyFieldName()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => s.Name);
            var rebound = MemberProjectionExpressionVisitor.Rebind(typeof(Sample), validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Equal(1, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsSinglePropertyNestedFieldName()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => s.Nested.Name);
            var rebound = MemberProjectionExpressionVisitor.Rebind(typeof(Sample), validMapping, source.Expression);

            Assert.Contains("nested.name", rebound.Collected);
            Assert.Equal(1, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsAnonymousProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => new { s.Name, s.Id, score = ElasticFields.Score, nestedName = s.Nested.Name });
            var rebound = MemberProjectionExpressionVisitor.Rebind(typeof(Sample), validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Contains("id", rebound.Collected);
            Assert.Contains("_score", rebound.Collected);
            Assert.Contains("nested.name", rebound.Collected);
            Assert.Equal(4, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsTupleCreateProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => Tuple.Create(s.Name, s.Id, ElasticFields.Score, s.Nested.Name));
            var rebound = MemberProjectionExpressionVisitor.Rebind(typeof(Sample), validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Contains("id", rebound.Collected);
            Assert.Contains("_score", rebound.Collected);
            Assert.Contains("nested.name", rebound.Collected);
            Assert.Equal(4, rebound.Collected.Count());
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsSimpleTokenFromDictionaryKeyFound()
        {
            var expected = new Sample { Id = "T-900", Name = "Cameron" };
            const string key = "Summer";
            var dictionary = new Dictionary<string, JToken> { { key, JToken.FromObject(expected) } };

            var actual = (Sample)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, key, typeof(Sample));

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsSingleItemInArrayFromDictionaryKeyFound()
        {
            const string expected = "Cameron";
            var dictionary = new Dictionary<string, JToken> { { "fields", JToken.Parse("[ \"" + expected + "\" ]") } };

            var actual = MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "fields", typeof(string));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsArrayIfArrayDesiredFromDictionaryKeyFound()
        {
            var expected = new[] { "Cameron" };
            var dictionary = new Dictionary<string, JToken> { { "fields", JToken.Parse("[ \"" + expected[0] + "\" ]") } };

            var actual = MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "fields", expected.GetType());

            Assert.IsType(expected.GetType(), actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDictionaryValueOrDefaultReturnsDefaultObjectIfDictionaryIsNull()
        {
            var actual = (DateTime)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(null, "Any", typeof(DateTime));

            Assert.Equal(default(DateTime), actual);
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
