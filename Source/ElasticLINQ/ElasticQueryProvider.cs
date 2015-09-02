// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Response.Model;
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
    /// Query provider implementation for Elasticsearch.
    /// </summary>
    public sealed class ElasticQueryProvider : IQueryProvider
    {
        private readonly ElasticRequestProcessor requestProcessor;

	    /// <summary>
	    /// Create a new ElasticQueryProvider for a given connection, mapping, log, retry policy and field prefix.
	    /// </summary>
	    /// <param name="connection">Connection to use to connect to Elasticsearch.</param>
	    /// <param name="mapping">A mapping to specify how queries and results are translated.</param>
	    /// <param name="log">A log to receive any information or debugging messages.</param>
	    /// <param name="retryPolicy">A policy to describe how to handle network issues.</param>
	    /// <param name="prefix">A string to use to prefix all Elasticsearch fields with.</param>
		public ElasticQueryProvider(IElasticConnection connection, IElasticMapping mapping, ILog log, IRetryPolicy retryPolicy, string prefix)
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

            requestProcessor = new ElasticRequestProcessor(connection, mapping, log, retryPolicy);
        }

		internal IElasticConnection Connection { get; private set; }

        internal ILog Log { get; private set; }

        internal IElasticMapping Mapping { get; private set; }

        internal string Prefix { get; private set; }

        internal IRetryPolicy RetryPolicy { get; private set; }

        /// <inheritdoc/>
        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            return new ElasticQuery<T>(this, expression);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public TResult Execute<TResult>(Expression expression)
        {
            Argument.EnsureNotNull("expression", expression);

            return (TResult)ExecuteInternal(expression);
        }

        /// <inheritdoc/>
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
                ElasticResponse response;
                if (translation.SearchRequest.Filter == ConstantCriteria.False)
                {
                    response = new ElasticResponse();
                }
                else
                {
                    response = AsyncHelper.RunSync(() => requestProcessor.SearchAsync(translation.SearchRequest));
                    if (response == null)
                        throw new InvalidOperationException("No HTTP response received.");
                }

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
