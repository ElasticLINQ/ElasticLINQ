// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Visitors;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
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

        [Fact]
        public void SelectIncludesConstructorWithNoMemberInitBindings()
        {
            Expression<Func<int, Robot>> expression = i => new Robot();

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType != ExpressionType.Parameter);

            Assert.Single(selectedBranches, s => s.NodeType == ExpressionType.New);
            Assert.Equal(1, selectedBranches.Count);
        }

        [Fact]
        public void SelectDoesNotIncludeMemberInitBranchesWhenTheyHaveNoIncludedBindings()
        {
            Expression<Func<int, Robot>> expression = i => new Robot { Id = i };

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType != ExpressionType.Parameter);

            Assert.Empty(selectedBranches);
        }

        [Fact]
        public void SelectDoesNotIncludeMemberInitBranchesWhenTheyPartiallyIncludedBindings()
        {
            Expression<Func<int, Robot>> expression = i => new Robot { Id = i, Name = "Hudzen-10" };

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType != ExpressionType.Parameter);

            Assert.Single(selectedBranches, s => s.NodeType == ExpressionType.Constant);
            Assert.Equal(1, selectedBranches.Count);
        }

        [Fact]
        public void SelectIncludeMemberInitBranchesWithAllBindings()
        {
            Expression<Func<int, Robot>> expression = i => new Robot { Id = 10, Name = "Hudzen-10" };

            var selectedBranches = BranchSelectExpressionVisitor.Select(expression, e => e.NodeType != ExpressionType.Parameter);

            Assert.Single(selectedBranches, s => s.NodeType == ExpressionType.MemberInit);
            Assert.Equal(3, selectedBranches.Count);
        }
    }
}