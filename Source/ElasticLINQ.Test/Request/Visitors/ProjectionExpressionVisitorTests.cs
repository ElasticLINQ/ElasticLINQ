// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class ProjectionExpressionVisitorTests
    {
        private class Sample { }
        private readonly ParameterExpression validParameter = Expression.Parameter(typeof(Sample), "s");
        private readonly IElasticMapping validMapping = new TrivialElasticMapping();

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(null, validMapping, Expression.Constant(1)));
        }

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfMappingIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(validParameter, null, Expression.Constant(1)));
        }

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(validParameter, validMapping, null));
        }
    }
}
