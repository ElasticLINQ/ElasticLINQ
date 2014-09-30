// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using ElasticLinq.Mapping;
using ElasticLinq.Retry;
using ElasticLinq.Utility;
using System.Linq;

namespace ElasticLinq
{
    /// <summary>
    /// Provides an entry point to easily create LINQ queries for Elasticsearch.
    /// </summary>
    public class ElasticContext : IElasticContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticContext"/> class.
        /// </summary>
        /// <param name="connection">The information on how to connect to the Elasticsearch server.</param>
        /// <param name="mapping">The object that helps map queries (optional, defaults to <see cref="TrivialElasticMapping"/>).</param>
        /// <param name="log">The object which logs information (optional, defaults to <see cref="NullLog"/>).</param>
        /// <param name="retryPolicy">The object which controls retry policy for the search (optional, defaults to <see cref="RetryPolicy"/>).</param>
        public ElasticContext(IElasticConnection connection, IElasticMapping mapping = null, ILog log = null, IRetryPolicy retryPolicy = null)
        {
            Argument.EnsureNotNull("connection", connection);

            Connection = connection;
            Mapping = mapping ?? new TrivialElasticMapping();
            Log = log ?? NullLog.Instance;
            RetryPolicy = retryPolicy ?? new RetryPolicy(Log);
        }

        /// <summary>
        /// Specifies the connection to the Elasticsearch server.
        /// </summary>
        public IElasticConnection Connection { get; private set; }

        /// <summary>
        /// The logging mechanism for diagnostics information.
        /// </summary>
        public ILog Log { get; private set; }

        /// <summary>
        /// The mapping to describe how objects and their properties are mapped to Elasticsearch.
        /// </summary>
        public IElasticMapping Mapping { get; private set; }

        /// <summary>
        /// The retry policy for handling networking issues.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; private set; }

        /// <inheritdoc/>
        public virtual IQueryable<T> Query<T>()
        {
            var prefix = Mapping.GetDocumentMappingPrefix(typeof(T));
            var provider = new ElasticQueryProvider(Connection, Mapping, Log, RetryPolicy, prefix);
            return new ElasticQuery<T>(provider);
        }
    }
}