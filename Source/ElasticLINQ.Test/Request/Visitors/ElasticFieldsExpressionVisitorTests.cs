// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class ElasticFieldsExpressionVisitorTests
    {
        private class Sample
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        private readonly IElasticMapping validMapping = new ElasticFieldsMappingWrapper(new TrivialElasticMapping());

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticFieldsExpressionVisitor.Rebind(null, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ElasticFieldsExpressionVisitor.Rebind(validMapping, null));
        }

        [Fact]
        public void RebindEntityToEntityIsUnchanged()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => f);
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);
            Assert.Same(source.Expression, rebound.Item1);
        }

        [Fact]
        public void RebindElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => ElasticFields.Score);
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound.Item1);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == rebound.Item2 && e.Member.Name == "_score");
        }

        [Fact]
        public void RebindAnonymousProjectionElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => new { ElasticFields.Score });
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound.Item1);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == rebound.Item2 && e.Member.Name == "_score");
            Assert.Contains(rebound.Item2, flattened);
        }

        [Fact]
        public void RebindTupleCreateMethodElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => Tuple.Create(ElasticFields.Score));
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound.Item1);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == rebound.Item2 && e.Member.Name == "_score");
            Assert.Contains(rebound.Item2, flattened);
        }

        [Fact]
        public void RebindAnonymousProjectionEntityAndElasticFieldsToMaterializationAndHitProperty()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => new { f, ElasticFields.Score });
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound.Item1);
            Assert.Single(flattened.OfType<MemberExpression>(), m => m.Expression == rebound.Item2 && m.Member.Name == "_score");
            Assert.Contains(rebound.Item2, flattened);

            var entityParameter = flattened.OfType<ParameterExpression>().FirstOrDefault(p => p.Name == "f" && p.Type == typeof(Sample));
            Assert.NotNull(entityParameter);
            Assert.Single(flattened.OfType<NewExpression>(), e => e.Arguments.Contains(entityParameter));
        }

        [Fact]
        public void RebindTupleCreateMethodEntityAndElasticFieldsToMaterializationAndHitProperty()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => Tuple.Create(ElasticFields.Id, f));
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound.Item1);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == rebound.Item2 && e.Member.Name == "_id");
            Assert.Contains(rebound.Item2, flattened);

            var entityParameter = flattened.OfType<ParameterExpression>().FirstOrDefault(p => p.Name == "f" && p.Type == typeof(Sample));
            Assert.NotNull(entityParameter);
            Assert.Single(flattened.OfType<MethodCallExpression>(), e => e.Arguments.Contains(entityParameter));
        }

        [Fact]
        public void RebindWithNonElasticMemberIsUnchanged()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => f.Name);
            var rebound = ElasticFieldsExpressionVisitor.Rebind(validMapping, source.Expression);

            var memberExpression = FlatteningExpressionVisitor.Flatten(rebound.Item1).OfType<MemberExpression>().FirstOrDefault();
            Assert.NotNull(memberExpression);
            Assert.Equal(memberExpression.Member.Name, "Name");
            Assert.IsAssignableFrom<ParameterExpression>(memberExpression.Expression);
            Assert.Equal("f", ((ParameterExpression)memberExpression.Expression).Name);
        }
    }
}