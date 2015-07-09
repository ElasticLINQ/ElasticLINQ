// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class PartialEvaluatorTests
    {
        readonly FakeQuery<Sample> sampleQuery = new FakeQuery<Sample>(new FakeQueryProvider());

        [Fact]
        public void ShouldNotEvaluateParameters()
        {
            var result = PartialEvaluator.ShouldEvaluate(Expression.Parameter(typeof(PartialEvaluatorTests)));

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotEvaluateLambdas()
        {
            Expression<Func<double>> getScoreField = () => ElasticFields.Score;
            var result = PartialEvaluator.ShouldEvaluate(Expression.Lambda(getScoreField));

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotEvaluateElasticFieldProperties()
        {
            Expression<Func<double>> getScoreField = () => ElasticFields.Score;
            var result = PartialEvaluator.ShouldEvaluate(getScoreField);

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotEvaluateQueryableExtensionMethods()
        {
            var result = PartialEvaluator.ShouldEvaluate(sampleQuery.Skip(0).Expression);

            Assert.False(result);
        }

        [Fact]
        public void ShouldNotEvaluateElasticQueryableMethods()
        {
            var result = PartialEvaluator.ShouldEvaluate(sampleQuery.OrderByScore().Expression);

            Assert.False(result);
        }

        class Sample { }
    }
}