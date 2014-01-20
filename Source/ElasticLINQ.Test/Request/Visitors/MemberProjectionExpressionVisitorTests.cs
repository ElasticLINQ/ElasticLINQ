// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
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
        private readonly ParameterExpression validParameter = Expression.Parameter(typeof(Sample), "s");
        private readonly IElasticMapping validMapping = new TrivialElasticMapping();

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind(null, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => MemberProjectionExpressionVisitor.Rebind(validMapping, null));
        }

        [Fact]
        public void RebindCollectsSinglePropertyFieldName()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => s.Name);
            var rebound = MemberProjectionExpressionVisitor.Rebind(validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Equal(1, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsAnonymousProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => new { s.Name, s.Id });
            var rebound = MemberProjectionExpressionVisitor.Rebind(validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Contains("id", rebound.Collected);
            Assert.Equal(2, rebound.Collected.Count());
        }

        [Fact]
        public void RebindCollectsTupleCreateProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => Tuple.Create(s.Name, s.Id));
            var rebound = MemberProjectionExpressionVisitor.Rebind(validMapping, source.Expression);

            Assert.Contains("name", rebound.Collected);
            Assert.Contains("id", rebound.Collected);
            Assert.Equal(2, rebound.Collected.Count());
        }

        [Fact]
        public void GetFieldValueReturnsTokenFromDictionaryIfKeyFound()
        {
            var expected = new Sample { Id = "T-900", Name = "Cameron" };
            const string key = "Summer";
            var dictionary = new Dictionary<string, JToken> { { key, JToken.FromObject(expected) } };

            var actual = (Sample)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, key, typeof(Sample));

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetFieldValueReturnsDefaultObjectIfKeyNotFoundForValueType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (int)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "Any", typeof(int));

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetFieldValueReturnsNullIfKeyNotFoundForReferenceType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (Sample)MemberProjectionExpressionVisitor.GetDictionaryValueOrDefault(dictionary, "Any", typeof(Sample));

            Assert.Null(actual);
        }
    }
}
