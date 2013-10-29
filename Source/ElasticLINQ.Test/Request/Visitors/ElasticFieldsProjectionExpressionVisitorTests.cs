// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class ElasticFieldsProjectionExpressionVisitorTests
    {
        private class Sample
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        private readonly ParameterExpression hitParameter = Expression.Parameter(typeof(Hit), "h");

        private readonly IElasticMapping validMapping = new ElasticFieldsMappingWrapper(new TrivialElasticMapping());

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ElasticFieldsProjectionExpressionVisitor.Rebind(null, validMapping, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, null, Expression.Constant(1)));
        }

        [Fact]
        public void RebindThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, null));
        }

        [Fact]
        public void RebindEntityToEntityIsUnchanged()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => f);
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);
            Assert.Same(source.Expression, rebound);
        }

        [Fact]
        public void RebindElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => ElasticFields.Score);
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == hitParameter && e.Member.Name == "_score");
        }

        [Fact]
        public void RebindAnonymousProjectionElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => new { ElasticFields.Score });
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == hitParameter && e.Member.Name == "_score");
            Assert.Contains(hitParameter, flattened);
        }

        [Fact]
        public void RebindTupleCreateMethodElasticFieldsToHitProperties()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => Tuple.Create(ElasticFields.Score));
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == hitParameter && e.Member.Name == "_score");
            Assert.Contains(hitParameter, flattened);
        }

        [Fact]
        public void RebindAnonymousProjectionEntityAndElasticFieldsToMaterializationAndHitProperty()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => new { f, ElasticFields.Score });
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound);
            Assert.Single(flattened.OfType<MemberExpression>(), m => m.Expression == hitParameter && m.Member.Name == "_score");
            Assert.Contains(hitParameter, flattened);

            var entityParameter = flattened.OfType<ParameterExpression>().FirstOrDefault(p => p.Name == "f" && p.Type == typeof(Sample));
            Assert.NotNull(entityParameter);
            Assert.Single(flattened.OfType<NewExpression>(), e => e.Arguments.Contains(entityParameter));
        }

        [Fact]
        public void RebindTupleCreateMethodEntityAndElasticFieldsToMaterializationAndHitProperty()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => Tuple.Create(ElasticFields.Id, f));
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var flattened = FlatteningExpressionVisitor.Flatten(rebound);
            Assert.Single(flattened.OfType<MemberExpression>(), e => e.Expression == hitParameter && e.Member.Name == "_id");
            Assert.Contains(hitParameter, flattened);

            var entityParameter = flattened.OfType<ParameterExpression>().FirstOrDefault(p => p.Name == "f" && p.Type == typeof(Sample));
            Assert.NotNull(entityParameter);
            Assert.Single(flattened.OfType<MethodCallExpression>(), e => e.Arguments.Contains(entityParameter));
        }

        [Fact]
        public void RebindWithNonElasticMemberIsUnchanged()
        {
            var source = new FakeQuery<Sample>(new FakeQueryProvider()).Select(f => f.Name);
            var rebound = ElasticFieldsProjectionExpressionVisitor.Rebind(hitParameter, validMapping, source.Expression);

            var memberExpression = FlatteningExpressionVisitor.Flatten(rebound).OfType<MemberExpression>().FirstOrDefault();
            Assert.NotNull(memberExpression);
            Assert.Equal(memberExpression.Member.Name, "Name");
            Assert.IsAssignableFrom<ParameterExpression>(memberExpression.Expression);
            Assert.Equal("f", ((ParameterExpression)memberExpression.Expression).Name);
        }
    }
}