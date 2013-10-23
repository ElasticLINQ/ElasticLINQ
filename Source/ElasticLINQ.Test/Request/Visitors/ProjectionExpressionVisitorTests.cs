// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class ProjectionExpressionVisitorTests
    {
        private class Sample { }

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(null, new TrivialElasticMapping(), Expression.Constant(1)));
        }

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfMappingIsNull()
        {
            var validParameter = Expression.Parameter(typeof(Sample), "s");

            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(validParameter, null, Expression.Constant(1)));
        }

        [Fact]
        public void ProjectColumnsThrowsArgumentNullExceptionIfSelectorIsNull()
        {
            var validParameter = Expression.Parameter(typeof(Sample), "s");

            Assert.Throws<ArgumentNullException>(() => ProjectionExpressionVisitor
                .ProjectColumns(validParameter, new TrivialElasticMapping(), null));
        }
    }
}
