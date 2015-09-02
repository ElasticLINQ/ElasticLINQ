using System;
using System.Threading.Tasks;
using ElasticLinq.Logging;
using ElasticLinq.Request;
using ElasticLinq.Response.Model;

namespace ElasticLinq
{
	/// <summary>
	/// The interface all clients which make requests to elastic search must implement
	/// </summary>
	public interface IElasticConnection
	{
		/// <summary>
		/// The name of the index on the Elasticsearch server.
		/// </summary>
		string Index { get; }

		/// <summary>
		/// Additional options that specify how this connection should behave.
		/// </summary>
		ElasticConnectionOptions Options { get; }

		/// <summary>
		/// How long to wait for a response to a network request before
		/// giving up.
		/// </summary>
		TimeSpan Timeout { get; }

		/// <summary>
		/// The Uri that specifies the public endpoint for the server.
		/// </summary>
		/// <example>http://myserver.example.com:9200</example>
		Uri Endpoint { get; }

		/// <summary>
		/// Issues search requests to elastic search
		/// </summary>
		/// <param name="searchIndex">The elastic search index</param>
		/// <param name="document">The elastic search document</param>
		/// <param name="body">The request body</param>
		/// <param name="searchRequest">The search request settings</param>
		/// <param name="log">The logging mechanism for diagnostic information.</param>
		/// <returns>An elastic response</returns>
		Task<ElasticResponse> Search(
			string searchIndex,
			string document,
			string body,
			SearchRequest searchRequest,
			ILog log);
	}
}
