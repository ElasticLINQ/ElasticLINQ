// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using ElasticLinq.Test.TestSupport;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Test.Request.Visitors.ElasticQueryTranslation
{
    public class ElasticQueryTranslationTestsBase
    {
        protected static readonly ElasticConnection Connection = new ElasticConnection(new Uri("http://localhost"));
        protected static readonly IElasticMapping Mapping = new TrivialElasticMapping();
        protected static readonly IElasticMapping CouchMapping = new CouchbaseElasticMapping();
        protected static readonly ILog Log = NullLog.Instance;
        protected static readonly IRetryPolicy RetryPolicy = NullRetryPolicy.Instance;
        protected static readonly ElasticQueryProvider SharedProvider = new ElasticQueryProvider(Connection, Mapping, Log, RetryPolicy);

        protected static IQueryable<Robot> Robots
        {
            get { return new ElasticQuery<Robot>(SharedProvider); }
        }

        protected static Expression MakeQueryableExpression<TSource>(string name, IQueryable<TSource> source, params Expression[] parameters)
        {
            parameters = parameters ?? new Expression[] { };

            var method = MakeQueryableMethod<TSource>(name, parameters.Length + 1);
            return Expression.Call(method, new[] { source.Expression }.Concat(parameters).ToArray());
        }

        protected static Expression MakeQueryableExpression<TSource, TResult>(IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, TResult>> operation)
        {
            var methodCall = (MethodCallExpression)operation.Body;
            return Expression.Call(methodCall.Method, new[] { source.Expression }.Concat(methodCall.Arguments.Skip(1)).ToArray());
        }

        protected static MethodInfo MakeQueryableMethod<TSource>(string name, int parameterCount)
        {
            return typeof(Queryable).FindMembers
                (MemberTypes.Method,
                    BindingFlags.Static | BindingFlags.Public,
                    (info, criteria) => info.Name.Equals(criteria), name)
                .OfType<MethodInfo>()
                .Single(a => a.GetParameters().Length == parameterCount)
                .MakeGenericMethod(typeof(TSource));
        }
    }
}