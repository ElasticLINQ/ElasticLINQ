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
    public class ProjectionExpressionVisitorTests
    {
        private class Sample
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }
        private readonly ParameterExpression validParameter = Expression.Parameter(typeof(Sample), "s");
        private readonly IElasticMapping validMapping = new TrivialElasticMapping();

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor.Rebind(null, validMapping, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor.Rebind(validParameter, null, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor.Rebind(validParameter, validMapping, null));
        }

        [Fact]
        public void RebindCollectsSinglePropertyFieldName()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => s.Name);
            var hitParameter = Expression.Parameter(typeof(Hit));
            var rebound = ProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            Assert.Contains("name", rebound.FieldNames);
            Assert.Equal(1, rebound.FieldNames.Count());
        }

        [Fact]
        public void RebindCollectsAnonymousProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => new { s.Name, s.Id });
            var hitParameter = Expression.Parameter(typeof(Hit));
            var rebound = ProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            Assert.Contains("name", rebound.FieldNames);
            Assert.Contains("id", rebound.FieldNames);
            Assert.Equal(2, rebound.FieldNames.Count());
        }

        [Fact]
        public void RebindCollectsTupleCreateProjectionPropertiesFieldNames()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(s => Tuple.Create(s.Name, s.Id));
            var hitParameter = Expression.Parameter(typeof(Hit));
            var rebound = ProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            Assert.Contains("name", rebound.FieldNames);
            Assert.Contains("id", rebound.FieldNames);
            Assert.Equal(2, rebound.FieldNames.Count());
        }

        [Fact]
        public void GetFieldValueReturnsTokenFromDictionaryIfKeyFound()
        {
            var expected = new Sample { Id = "T-900", Name = "Cameron" };
            const string key = "Summer";
            var dictionary = new Dictionary<string, JToken> { { key, JToken.FromObject(expected) } };

            var actual = (Sample) ProjectionExpressionVisitor.GetFieldValue(dictionary, key, typeof(Sample));

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetFieldValueReturnsDefaultObjectIfKeyNotFoundForValueType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (int)ProjectionExpressionVisitor.GetFieldValue(dictionary, "Any", typeof(int));

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetFieldValueReturnsNullIfKeyNotFoundForReferenceType()
        {
            var dictionary = new Dictionary<string, JToken>();

            var actual = (Sample)ProjectionExpressionVisitor.GetFieldValue(dictionary, "Any", typeof(Sample));

            Assert.Null(actual);
        }
    }
}
