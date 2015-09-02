﻿using System;
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

		private readonly string index;
		private readonly TimeSpan timeout;
		private readonly ElasticConnectionOptions options;

		/// <summary>
		/// Create a new BaseElasticConnection with the given parameters for internal testing.
		/// </summary>
		/// <param name="timeout">TimeSpan to wait for network responses before failing (optional, defaults to 10 seconds).</param>
		/// <param name="index">Name of the index to use on the server (optional).</param>
		/// <param name="options">Additional options that specify how this connection should behave.</param>
		protected BaseElasticConnection(string index = null, TimeSpan? timeout = null, ElasticConnectionOptions options = null)
        {
            if (timeout.HasValue)
                Argument.EnsurePositive("value", timeout.Value);
            if (index != null)
                Argument.EnsureNotBlank("index", index);

            this.index = index;
            this.options = options ?? new ElasticConnectionOptions();
            this.timeout = timeout ?? defaultTimeout;
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
		/// <param name="body">The request body</param>
		/// <param name="searchRequest">The search request settings</param>
		/// <param name="log">The logging mechanism for diagnostic information.</param>
		/// <returns>An elastic response</returns>
		public abstract Task<ElasticResponse> Search(string body, SearchRequest searchRequest, ILog log);

		/// <summary>
		/// Gets the uri of the search
		/// </summary>
		/// <param name="searchRequest">The search request settings</param>
		/// <returns>The uri of the search</returns>
		public abstract Uri GetSearchUri(SearchRequest searchRequest);
	}
}
