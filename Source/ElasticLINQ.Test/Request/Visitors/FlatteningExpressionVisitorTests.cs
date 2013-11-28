// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    public class FlatteningExpressionVisitorTests
    {
        [Fact]
        public void FlattenReturnsListOfAllExpressionNodes()
        {
            Expression<Func<int, bool>> sampleExpression = a => a > 2;

            var flattened = FlatteningExpressionVisitor.Flatten(sampleExpression);

            Assert.Equal(ExpressionType.Lambda, flattened[0].NodeType);
            Assert.Equal(ExpressionType.GreaterThan, flattened[1].NodeType);
            Assert.Equal(ExpressionType.Parameter, flattened[2].NodeType);
            Assert.Equal(ExpressionType.Constant, flattened[3].NodeType);
            Assert.Equal(ExpressionType.Parameter, flattened[4].NodeType);

            Assert.Equal(5, flattened.Count);
        }
    }
}