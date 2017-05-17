// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest.WhereTests
{
    public class PredicateWhereTests
    {
        static readonly IQueryable<WebUser> webUsers = new Data().Elastic<WebUser>();

        [Fact]
        public void CanUseSingleWherePredicate()
        {
            var results = webUsers.Where(WilsonsIdUnder20());

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void CanChainMultipleWherePredicatesForAnd()
        {
            var results = webUsers.Where(WilsonsIdUnder20()).Where(IdUnder5());

            Assert.Equal(1, results.Count());
        }

        [Fact]
        public void CanCombineMultipleWherePredicatesWithOrInvoke()
        {
            var results = webUsers.Where(Or.Combine(WilsonsIdUnder20(), w => w.Id < 5));

            Assert.Equal(5, results.Count());
        }

        private static Expression<Func<WebUser, bool>> WilsonsIdUnder20()
        {
            return w => w.Surname == "Wilson" && w.Id < 20;
        }

        private static Expression<Func<WebUser, bool>> IdUnder5()
        {
            return w => w.Id < 5;
        }
    }

    class Or
    {
        public static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var combined = new ParameterReplacer(parameter).Visit(Expression.OrElse(left.Body, right.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        class ParameterReplacer : ExpressionVisitor
        {
            readonly ParameterExpression parameter;

            internal ParameterReplacer(ParameterExpression parameter)
            {
                this.parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return parameter;
            }
        }
    }
}