// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request;
using ElasticLinq.Request.Visitors;
using ElasticLinq.Utility;
using System;
using System.IO;
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
        private readonly ElasticConnection connection;
        private readonly IElasticMapping mapping;

        public TextWriter Log { get; set; }

        public ElasticQueryProvider(ElasticConnection connection, IElasticMapping mapping)
        {
            Argument.EnsureNotNull("connection", connection);
            Argument.EnsureNotNull("mapping", mapping);

            this.connection = connection;
            this.mapping = mapping;
        }

        internal ElasticConnection Connection
        {
            get { return connection; }
        }

        internal IElasticMapping Mapping
        {
            get { return mapping; }
        }

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
            var translation = ElasticQueryTranslator.Translate(mapping, expression);
            var elementType = TypeHelper.GetSequenceElementType(expression.Type);

            if (Log != null)
                Log.WriteLine("Type is " + elementType);

            try
            {
                var response = AsyncHelper.RunSync(() => new ElasticRequestProcessor(connection, Log).SearchAsync(translation.SearchRequest));
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