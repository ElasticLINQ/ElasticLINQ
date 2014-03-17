// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ElasticLinq
{
    /// <summary>
    /// Query provider implementation for ElasticSearch.
    /// </summary>
    public sealed class ElasticQueryProvider : IQueryProvider
    {
        public ElasticQueryProvider(ElasticConnection connection, IElasticMapping mapping, ILog log, IRetryPolicy retryPolicy, string prefix)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);
            Argument.EnsureNotNull("log", log);
            Argument.EnsureNotNull("retryPolicy", retryPolicy);

            Connection = connection;
            Mapping = mapping;
            Log = log;
            RetryPolicy = retryPolicy;
            Prefix = prefix;
        }

        internal ElasticConnection Connection { get; private set; }

        internal ILog Log { get; private set; }

        internal IElasticMapping Mapping { get; private set; }

        internal string Prefix { get; private set; }

        internal IRetryPolicy RetryPolicy { get; private set; }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            return new ElasticQuery<T>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            var elementType = TypeHelper.GetSequenceElementType(expression.Type);
            var queryType = typeof(ElasticQuery<>).MakeGenericType(elementType);
            try
            {
                return (IQueryable)Activator.CreateInstance(queryType, new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;  // Never called, as the above code re-throws
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            return (TResult)ExecuteInternal(expression);
        }

        public object Execute(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            return ExecuteInternal(expression);
        }

        private object ExecuteInternal(Expression expression)
        {
            var translation = ElasticQueryTranslator.Translate(Mapping, Prefix, expression);
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);

            Log.Debug(null, null, "Executing query against type {0}", elementType);

            try
            {
                var response = AsyncHelper.RunSync(() => new ElasticRequestProcessor(Connection, Mapping, Log, RetryPolicy).SearchAsync(translation.SearchRequest));
                if (response == null)
                    throw new InvalidOperationException("No HTTP response received.");

                return translation.Materializer.Materialize(response);
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;  // Never called, as the above code re-throws
            }
        }
    }
}
