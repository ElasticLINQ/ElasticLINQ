// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Request.Visitors;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLINQ.Test.Request.Visitors
{
    public class BranchSelectExpressionVisitorTests
    {
        [Fact]
        public void SelectFindsSimpleNodeTypes()
        {
            Expression<Func<bool>> expression = () => new DateTime(2002, 01, 02) < DateTime.Now;

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType == ExpressionType.MemberAccess);

            Assert.Single(selectedBranches, s => s.NodeType == ExpressionType.MemberAccess);
            Assert.Equal(1, selectedBranches.Count);
        }

        [Fact]
        public void SelectFindsBranchesWithoutParameterReferences()
        {
            Expression<Func<int, DateTime>> expression = a => DateTime.Now.Subtract(TimeSpan.FromSeconds(a));

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType != ExpressionType.Parameter);

            Assert.Single(selectedBranches, s => s.NodeType == ExpressionType.MemberAccess);
            Assert.Equal(1, selectedBranches.Count);
        }
    }
}