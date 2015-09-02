using System;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;
using ElasticLinq.Utility;

namespace ElasticLinq
{
	/// <summary>
	/// Specifies connection parameters for Elasticsearch.
	/// </summary>
	public abstract class BaseElasticConnection : IElasticConnection
	{
		private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);

		private readonly Uri endpoint;
		private readonly string index;
		private readonly TimeSpan timeout;
		private readonly ElasticConnectionOptions options;

		/// <summary>
		/// Create a new BaseElasticConnection with the given parameters for internal testing.
		/// </summary>
		/// <param name="endpoint">The URL endpoint of the Elasticsearch server.</param>
		/// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
		/// <param name="index">Name of the index to use on the server (optional).</param>
		/// <param name="options">Additional options that specify how this connection should behave.</param>
		protected BaseElasticConnection(Uri endpoint, string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
        {
            Argument.EnsureNotNull("endpoint", endpoint);

            if (timeout.HasValue)
                Argument.EnsurePositive("value", timeout.Value);
            if (index != null)
                Argument.EnsureNotBlank("index", index);

            this.endpoint = endpoint;
            this.index = index;
            this.options = options ?? new ElasticConnectionOptions();
            this.timeout = timeout ?? defaultTimeout;
        }

		/// <summary>
		/// The Uri that specifies the public endpoint for the server.
		/// </summary>
		/// <example>http://myserver.example.com:9200</example>
		public Uri Endpoint
		{
			get { return endpoint; }
		}

		/// <summary>
		/// The name of the index on the Elasticsearch server.
		/// </summary>
		/// <example>northwind</example>
		public string Index
		{
			get { return index; }
		}

		/// <summary>
		/// How long to wait for a response to a network request before
		/// giving up.
		/// </summary>
		public TimeSpan Timeout
		{
			get { return timeout; }
		}

		/// <summary>
		/// Additional options that specify how this connection should behave.
		/// </summary>
		public ElasticConnectionOptions Options
		{
			get { return options; }
		}

		/// <summary>
		/// Issues search requests to elastic search
		/// </summary>
		/// <param name="searchIndex">The elastic search index</param>
		/// <param name="document">The elastic search document</param>
		/// <param name="body">The request body</param>
		/// <param name="searchRequest">The search request settings</param>
		/// <param name="log">The logging mechanism for diagnostic information.</param>
		/// <returns>An elastic response</returns>
		public abstract Task<ElasticResponse> Search(string searchIndex, string document, string body, SearchRequest searchRequest, ILog log);
	}
}
