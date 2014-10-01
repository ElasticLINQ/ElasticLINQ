namespace ElasticApi.Requests
{
	using System;
	using System.Collections.Generic;
	using Attributes;

    /// <summary>
    /// <para>Request for get api</para>
	/// <para>Path: /{index}/{type}/{id}</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /{index}/{type}/{id}</para>
    /// </summary>
	public class GetRequest
	{
        /// <summary>
        /// <para>The document ID</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "id", Position = 3, Required = true)]
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = 1, Required = true)]
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document (use `_all` to fetch the first document matching the ID across all types)</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "type", Position = 2, Required = true)]
		public string Type { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields to return in the response</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		[ApiParam(Name = "fields")]
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>The ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		[ApiParam(Name = "parent")]
		public string Parent { get; set; }
        /// <summary>
        /// <para>Specify the node or shard the operation should be performed on (default: random)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: preference</para>
        /// </summary>
		[ApiParam(Name = "preference")]
		public string Preference { get; set; }
        /// <summary>
        /// <para>Specify whether to perform the operation in realtime or search mode</para>
		/// <para>Type: parameter</para>
        /// <para>Name: realtime</para>
        /// </summary>
		[ApiParam(Name = "realtime")]
		public Nullable<bool> Realtime { get; set; }
        /// <summary>
        /// <para>Refresh the shard containing the document before performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		[ApiParam(Name = "refresh")]
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		[ApiParam(Name = "routing")]
		public string Routing { get; set; }
        /// <summary>
        /// <para>True or false to return the _source field or not, or a list of fields to return</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source</para>
        /// </summary>
		[ApiParam(Name = "_source")]
		public IEnumerable<string> Source { get; set; }
        /// <summary>
        /// <para>A list of fields to exclude from the returned _source field</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source_exclude</para>
        /// </summary>
		[ApiParam(Name = "_source_exclude")]
		public IEnumerable<string> SourceExclude { get; set; }
        /// <summary>
        /// <para>A list of fields to extract and return from the _source field</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source_include</para>
        /// </summary>
		[ApiParam(Name = "_source_include")]
		public IEnumerable<string> SourceInclude { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		[ApiParam(Name = "version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
		[ApiParam(Name = "version_type")]
		public string VersionType { get; set; }
	}
    /// <summary>
    /// <para>Request for index api</para>
	/// <para>Path: /{index}/{type}/{id}</para>
	/// <para>Methods: PUT, POST</para>
	/// <para>Paths: /{index}/{type}/{id}, /{index}/{type}</para>
    /// </summary>
	public class IndexRequest
	{
        /// <summary>
        /// <para>Document ID</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "id", Position = 3, Required = false)]
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = 1, Required = true)]
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "type", Position = 2, Required = true)]
		public string Type { get; set; }
        /// <summary>
        /// <para>Explicit write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		[ApiParam(Name = "consistency")]
		public string Consistency { get; set; }
        /// <summary>
        /// <para>Explicit operation type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: op_type</para>
        /// </summary>
		[ApiParam(Name = "op_type")]
		public string OpType { get; set; }
        /// <summary>
        /// <para>ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		[ApiParam(Name = "parent")]
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		[ApiParam(Name = "refresh")]
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		[ApiParam(Name = "replication")]
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		[ApiParam(Name = "routing")]
		public string Routing { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit timestamp for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timestamp</para>
        /// </summary>
		[ApiParam(Name = "timestamp")]
		public Nullable<TimeSpan> Timestamp { get; set; }
        /// <summary>
        /// <para>Expiration time for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: ttl</para>
        /// </summary>
		[ApiParam(Name = "ttl")]
		public Nullable<TimeSpan> Ttl { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		[ApiParam(Name = "version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
		[ApiParam(Name = "version_type")]
		public string VersionType { get; set; }
        /// <summary>
        /// <para>The document</para>
        /// </summary>
		[ApiBody]
		public object Body { get; set; }
	}
    /// <summary>
    /// <para>Request for delete api</para>
	/// <para>Path: /{index}/{type}/{id}</para>
	/// <para>Methods: DELETE</para>
	/// <para>Paths: /{index}/{type}/{id}</para>
    /// </summary>
	public class DeleteRequest
	{
        /// <summary>
        /// <para>The document ID</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "id", Position = 3, Required = true)]
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = 1, Required = true)]
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "type", Position = 2, Required = true)]
		public string Type { get; set; }
        /// <summary>
        /// <para>Specific write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		[ApiParam(Name = "consistency")]
		public string Consistency { get; set; }
        /// <summary>
        /// <para>ID of parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		[ApiParam(Name = "parent")]
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		[ApiParam(Name = "refresh")]
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		[ApiParam(Name = "replication")]
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		[ApiParam(Name = "routing")]
		public string Routing { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		[ApiParam(Name = "version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
		[ApiParam(Name = "version_type")]
		public string VersionType { get; set; }
	}
    /// <summary>
    /// <para>Request for update api</para>
	/// <para>Path: /{index}/{type}/{id}/_update</para>
	/// <para>Methods: POST</para>
	/// <para>Paths: /{index}/{type}/{id}/_update</para>
    /// </summary>
	public class UpdateRequest
	{
        /// <summary>
        /// <para>Document ID</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "id", Position = 3, Required = true)]
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = 1, Required = true)]
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		[ApiRoute(Part = "type", Position = 2, Required = true)]
		public string Type { get; set; }
        /// <summary>
        /// <para>Explicit write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		[ApiParam(Name = "consistency")]
		public string Consistency { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields to return in the response</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		[ApiParam(Name = "fields")]
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>The script language (default: groovy)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: lang</para>
        /// </summary>
		[ApiParam(Name = "lang")]
		public string Lang { get; set; }
        /// <summary>
        /// <para>ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		[ApiParam(Name = "parent")]
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		[ApiParam(Name = "refresh")]
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		[ApiParam(Name = "replication")]
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specify how many times should the operation be retried when a conflict occurs (default: 0)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: retry_on_conflict</para>
        /// </summary>
		[ApiParam(Name = "retry_on_conflict")]
		public Nullable<long> RetryOnConflict { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		[ApiParam(Name = "routing")]
		public string Routing { get; set; }
        /// <summary>
        /// <para>The URL-encoded script definition (instead of using request body)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: script</para>
        /// </summary>
		[ApiParam(Name = "script")]
		public string Script { get; set; }
        /// <summary>
        /// <para>The id of a stored script</para>
		/// <para>Type: parameter</para>
        /// <para>Name: script_id</para>
        /// </summary>
		[ApiParam(Name = "script_id")]
		public string ScriptId { get; set; }
        /// <summary>
        /// <para>True if the script referenced in script or script_id should be called to perform inserts - defaults to false</para>
		/// <para>Type: parameter</para>
        /// <para>Name: scripted_upsert</para>
        /// </summary>
		[ApiParam(Name = "scripted_upsert")]
		public Nullable<bool> ScriptedUpsert { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit timestamp for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timestamp</para>
        /// </summary>
		[ApiParam(Name = "timestamp")]
		public Nullable<TimeSpan> Timestamp { get; set; }
        /// <summary>
        /// <para>Expiration time for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: ttl</para>
        /// </summary>
		[ApiParam(Name = "ttl")]
		public Nullable<TimeSpan> Ttl { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		[ApiParam(Name = "version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
		[ApiParam(Name = "version_type")]
		public string VersionType { get; set; }
        /// <summary>
        /// <para>The request definition using either `script` or partial `doc`</para>
        /// </summary>
		[ApiBody]
		public object Body { get; set; }
	}
}

namespace ElasticApi.Responses
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;

    /// <summary>
    /// <para>Response for get api</para>
    /// </summary>
	public class GetResponse
	{
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _index</para>
        /// </summary>
		[JsonProperty("_index")]
		public string Index { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _type</para>
        /// </summary>
		[JsonProperty("_type")]
		public string Type { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _id</para>
        /// </summary>
		[JsonProperty("_id")]
		public string Id { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _version</para>
        /// </summary>
		[JsonProperty("_version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: found</para>
        /// </summary>
		[JsonProperty("found")]
		public Nullable<bool> Found { get; set; }
	}
    /// <summary>
    /// <para>Response for index api</para>
    /// </summary>
	public class IndexResponse
	{
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _index</para>
        /// </summary>
		[JsonProperty("_index")]
		public string Index { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _type</para>
        /// </summary>
		[JsonProperty("_type")]
		public string Type { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _id</para>
        /// </summary>
		[JsonProperty("_id")]
		public string Id { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _version</para>
        /// </summary>
		[JsonProperty("_version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: created</para>
        /// </summary>
		[JsonProperty("created")]
		public Nullable<bool> Created { get; set; }
	}
    /// <summary>
    /// <para>Response for delete api</para>
    /// </summary>
	public class DeleteResponse
	{
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _index</para>
        /// </summary>
		[JsonProperty("_index")]
		public string Index { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _type</para>
        /// </summary>
		[JsonProperty("_type")]
		public string Type { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _id</para>
        /// </summary>
		[JsonProperty("_id")]
		public string Id { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _version</para>
        /// </summary>
		[JsonProperty("_version")]
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>todo</para>
		/// <para>Type: parameter</para>
        /// <para>Name: found</para>
        /// </summary>
		[JsonProperty("found")]
		public Nullable<bool> Found { get; set; }
	}
    /// <summary>
    /// <para>Response for update api</para>
    /// </summary>
	public class UpdateResponse
	{
	}
}

namespace ElasticApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Requests;
    using Responses;

	public static class Elastic
	{
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-get.html
		/// </summary>
		/// <param name="request">request input</param>
		public static GetResponse Get(IConnection connection, GetRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<GetResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-get.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<GetResponse> GetAsync(IConnection connection, GetRequest request)
		{
			//	TODO : async version
			return default(Task<GetResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-index_.html
		/// </summary>
		/// <param name="request">request input</param>
		public static IndexResponse Index(IConnection connection, IndexRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			object body = RequestHelper.GetBody(request);

			return connection.Put<IndexResponse>(path, parameters, body);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-index_.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<IndexResponse> IndexAsync(IConnection connection, IndexRequest request)
		{
			//	TODO : async version
			return default(Task<IndexResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-delete.html
		/// </summary>
		/// <param name="request">request input</param>
		public static DeleteResponse Delete(IConnection connection, DeleteRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Delete<DeleteResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-delete.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<DeleteResponse> DeleteAsync(IConnection connection, DeleteRequest request)
		{
			//	TODO : async version
			return default(Task<DeleteResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-update.html
		/// </summary>
		/// <param name="request">request input</param>
		public static UpdateResponse Update(IConnection connection, UpdateRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			object body = RequestHelper.GetBody(request);

			return connection.Post<UpdateResponse>(path, parameters, body);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/docs-update.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<UpdateResponse> UpdateAsync(IConnection connection, UpdateRequest request)
		{
			//	TODO : async version
			return default(Task<UpdateResponse>);
		}
	}
}
