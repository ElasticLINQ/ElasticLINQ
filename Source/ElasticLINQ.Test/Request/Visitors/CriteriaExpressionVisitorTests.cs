// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Utility;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ElasticLinq.Test.Request.Visitors
{
    // CriteriaExpressionVisitor is a base class that is used by other visitors
    // and is extensively test covered by those subclass tests. This test is for a
    // few error-case paths to ensure maximum code coverage.

    class SimpleCriteriaExpressionVisitor : CriteriaExpressionVisitor
    {
        public SimpleCriteriaExpressionVisitor(IElasticMapping mapping, string prefix)
            : base(mapping, prefix)
        {
        }

        public new Expression VisitElasticMethodsMethodCall(MethodCallExpression m)
        {
            return base.VisitElasticMethodsMethodCall(m);
        }

        public new Expression VisitEnumerableMethodCall(MethodCallExpression m)
        {
            return base.VisitEnumerableMethodCall(m);
        }

        public new Expression VisitMember(MemberExpression m)
        {
            return base.VisitMember(m);
        }
    }

    public static class CriteriaExpressionVisitorTests
    {
        private static readonly SimpleCriteriaExpressionVisitor visitor = new SimpleCriteriaExpressionVisitor(new TrivialElasticMapping(), "");

        [Fact]
        public static void VisitElasticMethodsMethodCallThrowsNotSupportedForUnknownMethods()
        {
            const string methodName = "VisitElasticMethodsMethodCallThrowsNotSupportedForUnknownMethods";
            var methodInfo = typeof(CriteriaExpressionVisitorTests).GetMethodInfo(m => m.Name == methodName);
            var callExpression = Expression.Call(null, methodInfo);

            var exception = Assert.Throws<NotSupportedException>(() => visitor.VisitElasticMethodsMethodCall(callExpression));
            Assert.Contains("ElasticMethods." + methodName, exception.Message);
        }

        [Fact]
        public static void VisitEnumerableMethodCallThrowsNotSupportedForUnknownMethods()
        {
            const string methodName = "VisitEnumerableMethodCallThrowsNotSupportedForUnknownMethods";
            var methodInfo = typeof(CriteriaExpressionVisitorTests).GetMethodInfo(m => m.Name == methodName);
            var callExpression = Expression.Call(null, methodInfo);

            var exception = Assert.Throws<NotSupportedException>(() => visitor.VisitEnumerableMethodCall(callExpression));
            Assert.Contains("Enumerable." + methodName, exception.Message);
        }

        [Fact]
        public static void VisitMemberThrowsNotSupportedForConstantExpression()
        {
            const string memberName = "Length";
            var memberInfo = typeof(string).GetMember(memberName)[0];
            var memberExpression = Expression.MakeMemberAccess(Expression.Constant(""), memberInfo);

            var exception = Assert.Throws<NotSupportedException>(() => visitor.VisitMember(memberExpression));
            Assert.Contains("String." + memberName, exception.Message);
            Assert.Contains("Constant", exception.Message);
        }
    }
}