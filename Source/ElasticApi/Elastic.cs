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
    /// <summary>
    /// <para>Request for cluster_health api</para>
	/// <para>Path: /_cluster/health</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/health, /_cluster/health/{index}</para>
    /// </summary>
	public class ClusterHealthRequest
	{
        /// <summary>
        /// <para>Limit the information returned to a specific index</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = -1, Required = false)]
		public string Index { get; set; }
        /// <summary>
        /// <para>Specify the level of detail for returned information</para>
		/// <para>Type: parameter</para>
        /// <para>Name: level</para>
        /// </summary>
		[ApiParam(Name = "level")]
		public string Level { get; set; }
        /// <summary>
        /// <para>Return local information, do not retrieve the state from master node (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: local</para>
        /// </summary>
		[ApiParam(Name = "local")]
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		[ApiParam(Name = "master_timeout")]
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of shards is active</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_active_shards</para>
        /// </summary>
		[ApiParam(Name = "wait_for_active_shards")]
		public Nullable<long> WaitForActiveShards { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of nodes is available</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_nodes</para>
        /// </summary>
		[ApiParam(Name = "wait_for_nodes")]
		public string WaitForNodes { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of relocating shards is finished</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_relocating_shards</para>
        /// </summary>
		[ApiParam(Name = "wait_for_relocating_shards")]
		public Nullable<long> WaitForRelocatingShards { get; set; }
        /// <summary>
        /// <para>Wait until cluster is in a specific state</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_status</para>
        /// </summary>
		[ApiParam(Name = "wait_for_status")]
		public string WaitForStatus { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_state api</para>
	/// <para>Path: /_cluster/state</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/state, /_cluster/state/{metric}, /_cluster/state/{metric}/{index}</para>
    /// </summary>
	public class ClusterStateRequest
	{
        /// <summary>
        /// <para>A comma-separated list of index names; use `_all` or empty string to perform the operation on all indices</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "index", Position = -1, Required = false)]
		public IEnumerable<string> Index { get; set; }
        /// <summary>
        /// <para>Limit the information returned to the specified metrics</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "metric", Position = -1, Required = false)]
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Return local information, do not retrieve the state from master node (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: local</para>
        /// </summary>
		[ApiParam(Name = "local")]
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Specify timeout for connection to master</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		[ApiParam(Name = "master_timeout")]
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		[ApiParam(Name = "flat_settings")]
		public Nullable<bool> FlatSettings { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_stats api</para>
	/// <para>Path: /_cluster/stats</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/stats, /_cluster/stats/nodes/{node_id}</para>
    /// </summary>
	public class ClusterStatsRequest
	{
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to limit the returned information; use `_local` to return information from the node you're connecting to, leave empty to get information from all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "node_id", Position = -1, Required = false)]
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		[ApiParam(Name = "flat_settings")]
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
		[ApiParam(Name = "human")]
		public Nullable<bool> Human { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_pending_tasks api</para>
	/// <para>Path: /_cluster/pending_tasks</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/pending_tasks</para>
    /// </summary>
	public class ClusterPendingTasksRequest
	{
        /// <summary>
        /// <para>Return local information, do not retrieve the state from master node (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: local</para>
        /// </summary>
		[ApiParam(Name = "local")]
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Specify timeout for connection to master</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		[ApiParam(Name = "master_timeout")]
		public Nullable<TimeSpan> MasterTimeout { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_reroute api</para>
	/// <para>Path: /_cluster/reroute</para>
	/// <para>Methods: POST</para>
	/// <para>Paths: /_cluster/reroute</para>
    /// </summary>
	public class ClusterRerouteRequest
	{
        /// <summary>
        /// <para>Simulate the operation only and return the resulting state</para>
		/// <para>Type: parameter</para>
        /// <para>Name: dry_run</para>
        /// </summary>
		[ApiParam(Name = "dry_run")]
		public Nullable<bool> DryRun { get; set; }
        /// <summary>
        /// <para>Return an explanation of why the commands can or cannot be executed</para>
		/// <para>Type: parameter</para>
        /// <para>Name: explain</para>
        /// </summary>
		[ApiParam(Name = "explain")]
		public Nullable<bool> Explain { get; set; }
        /// <summary>
        /// <para>Limit the information returned to the specified metrics. Defaults to all but metadata</para>
		/// <para>Type: parameter</para>
        /// <para>Name: metric</para>
        /// </summary>
		[ApiParam(Name = "metric")]
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		[ApiParam(Name = "master_timeout")]
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>The definition of `commands` to perform (`move`, `cancel`, `allocate`)</para>
        /// </summary>
		[ApiBody]
		public object Body { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_get_settings api</para>
	/// <para>Path: /_cluster/settings</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/settings</para>
    /// </summary>
	public class ClusterGetSettingsRequest
	{
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		[ApiParam(Name = "flat_settings")]
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		[ApiParam(Name = "master_timeout")]
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		[ApiParam(Name = "timeout")]
		public Nullable<TimeSpan> Timeout { get; set; }
	}
    /// <summary>
    /// <para>Request for cluster_put_settings api</para>
	/// <para>Path: /_cluster/settings</para>
	/// <para>Methods: PUT</para>
	/// <para>Paths: /_cluster/settings</para>
    /// </summary>
	public class ClusterPutSettingsRequest
	{
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		[ApiParam(Name = "flat_settings")]
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>The settings to be updated. Can be either `transient` or `persistent` (survives cluster restart).</para>
        /// </summary>
		[ApiBody]
		public object Body { get; set; }
	}
    /// <summary>
    /// <para>Request for nodes_stats api</para>
	/// <para>Path: /_nodes/stats</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_nodes/stats, /_nodes/{node_id}/stats, /_nodes/stats/{metric}, /_nodes/{node_id}/stats/{metric}, /_nodes/stats/{metric}/{index_metric}, /_nodes/{node_id}/stats/{metric}/{index_metric}</para>
    /// </summary>
	public class NodesStatsRequest
	{
        /// <summary>
        /// <para>Limit the information returned to the specified metrics</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "metric", Position = -1, Required = false)]
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Limit the information returned for `indices` metric to the specific index metrics. Isn't used if `indices` (or `all`) metric isn't specified.</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "index_metric", Position = -1, Required = false)]
		public IEnumerable<string> IndexMetric { get; set; }
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to limit the returned information; use `_local` to return information from the node you're connecting to, leave empty to get information from all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "node_id", Position = -1, Required = false)]
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` and `suggest` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: completion_fields</para>
        /// </summary>
		[ApiParam(Name = "completion_fields")]
		public IEnumerable<string> CompletionFields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fielddata_fields</para>
        /// </summary>
		[ApiParam(Name = "fielddata_fields")]
		public IEnumerable<string> FielddataFields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` and `completion` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		[ApiParam(Name = "fields")]
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of search groups for `search` index metric</para>
		/// <para>Type: parameter</para>
        /// <para>Name: groups</para>
        /// </summary>
		[ApiParam(Name = "groups")]
		public Nullable<bool> Groups { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
		[ApiParam(Name = "human")]
		public Nullable<bool> Human { get; set; }
        /// <summary>
        /// <para>Return indices stats aggregated at node, index or shard level</para>
		/// <para>Type: parameter</para>
        /// <para>Name: level</para>
        /// </summary>
		[ApiParam(Name = "level")]
		public string Level { get; set; }
        /// <summary>
        /// <para>A comma-separated list of document types for the `indexing` index metric</para>
		/// <para>Type: parameter</para>
        /// <para>Name: types</para>
        /// </summary>
		[ApiParam(Name = "types")]
		public IEnumerable<string> Types { get; set; }
	}
    /// <summary>
    /// <para>Request for nodes_info api</para>
	/// <para>Path: /_nodes</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_nodes, /_nodes/{node_id}, /_nodes/{metric}, /_nodes/{node_id}/{metric}</para>
    /// </summary>
	public class NodesInfoRequest
	{
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to limit the returned information; use `_local` to return information from the node you're connecting to, leave empty to get information from all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "node_id", Position = -1, Required = false)]
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>A comma-separated list of metrics you wish returned. Leave empty to return all.</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "metric", Position = -1, Required = false)]
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		[ApiParam(Name = "flat_settings")]
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
		[ApiParam(Name = "human")]
		public Nullable<bool> Human { get; set; }
	}
    /// <summary>
    /// <para>Request for nodes_hot_threads api</para>
	/// <para>Path: /_nodes/hot_threads</para>
	/// <para>Methods: GET</para>
	/// <para>Paths: /_cluster/nodes/hotthreads, /_cluster/nodes/hot_threads, /_cluster/nodes/{node_id}/hotthreads, /_cluster/nodes/{node_id}/hot_threads, /_nodes/hotthreads, /_nodes/hot_threads, /_nodes/{node_id}/hotthreads, /_nodes/{node_id}/hot_threads</para>
    /// </summary>
	public class NodesHotThreadsRequest
	{
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to limit the returned information; use `_local` to return information from the node you're connecting to, leave empty to get information from all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "node_id", Position = -1, Required = false)]
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>The interval for the second sampling of threads</para>
		/// <para>Type: parameter</para>
        /// <para>Name: interval</para>
        /// </summary>
		[ApiParam(Name = "interval")]
		public Nullable<TimeSpan> Interval { get; set; }
        /// <summary>
        /// <para>Number of samples of thread stacktrace (default: 10)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: snapshots</para>
        /// </summary>
		[ApiParam(Name = "snapshots")]
		public Nullable<long> Snapshots { get; set; }
        /// <summary>
        /// <para>Specify the number of threads to provide information for (default: 3)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: threads</para>
        /// </summary>
		[ApiParam(Name = "threads")]
		public Nullable<long> Threads { get; set; }
        /// <summary>
        /// <para>The type to sample (default: cpu)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: type</para>
        /// </summary>
		[ApiParam(Name = "type")]
		public string Type { get; set; }
	}
    /// <summary>
    /// <para>Request for nodes_shutdown api</para>
	/// <para>Path: /_shutdown</para>
	/// <para>Methods: POST</para>
	/// <para>Paths: /_shutdown, /_cluster/nodes/_shutdown, /_cluster/nodes/{node_id}/_shutdown</para>
    /// </summary>
	public class NodesShutdownRequest
	{
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to perform the operation on; use `_local` to perform the operation on the node you're connected to, leave empty to perform the operation on all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		[ApiRoute(Part = "node_id", Position = -1, Required = false)]
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>Set the delay for the operation (default: 1s)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: delay</para>
        /// </summary>
		[ApiParam(Name = "delay")]
		public Nullable<TimeSpan> Delay { get; set; }
        /// <summary>
        /// <para>Exit the JVM as well (default: true)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: exit</para>
        /// </summary>
		[ApiParam(Name = "exit")]
		public Nullable<bool> Exit { get; set; }
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
    /// <summary>
    /// <para>Response for cluster_health api</para>
    /// </summary>
	public class ClusterHealthResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_state api</para>
    /// </summary>
	public class ClusterStateResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_stats api</para>
    /// </summary>
	public class ClusterStatsResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_pending_tasks api</para>
    /// </summary>
	public class ClusterPendingTasksResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_reroute api</para>
    /// </summary>
	public class ClusterRerouteResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_get_settings api</para>
    /// </summary>
	public class ClusterGetSettingsResponse
	{
	}
    /// <summary>
    /// <para>Response for cluster_put_settings api</para>
    /// </summary>
	public class ClusterPutSettingsResponse
	{
	}
    /// <summary>
    /// <para>Response for nodes_stats api</para>
    /// </summary>
	public class NodesStatsResponse
	{
	}
    /// <summary>
    /// <para>Response for nodes_info api</para>
    /// </summary>
	public class NodesInfoResponse
	{
	}
    /// <summary>
    /// <para>Response for nodes_hot_threads api</para>
    /// </summary>
	public class NodesHotThreadsResponse
	{
	}
    /// <summary>
    /// <para>Response for nodes_shutdown api</para>
    /// </summary>
	public class NodesShutdownResponse
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
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-health.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterHealthResponse ClusterHealth(IConnection connection, ClusterHealthRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<ClusterHealthResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-health.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterHealthResponse> ClusterHealthAsync(IConnection connection, ClusterHealthRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterHealthResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-state.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterStateResponse ClusterState(IConnection connection, ClusterStateRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<ClusterStateResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-state.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterStateResponse> ClusterStateAsync(IConnection connection, ClusterStateRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterStateResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-stats.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterStatsResponse ClusterStats(IConnection connection, ClusterStatsRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<ClusterStatsResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-stats.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterStatsResponse> ClusterStatsAsync(IConnection connection, ClusterStatsRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterStatsResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-pending.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterPendingTasksResponse ClusterPendingTasks(IConnection connection, ClusterPendingTasksRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<ClusterPendingTasksResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-pending.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterPendingTasksResponse> ClusterPendingTasksAsync(IConnection connection, ClusterPendingTasksRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterPendingTasksResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-reroute.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterRerouteResponse ClusterReroute(IConnection connection, ClusterRerouteRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			object body = RequestHelper.GetBody(request);

			return connection.Post<ClusterRerouteResponse>(path, parameters, body);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-reroute.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterRerouteResponse> ClusterRerouteAsync(IConnection connection, ClusterRerouteRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterRerouteResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-update-settings.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterGetSettingsResponse ClusterGetSettings(IConnection connection, ClusterGetSettingsRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<ClusterGetSettingsResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-update-settings.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterGetSettingsResponse> ClusterGetSettingsAsync(IConnection connection, ClusterGetSettingsRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterGetSettingsResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-update-settings.html
		/// </summary>
		/// <param name="request">request input</param>
		public static ClusterPutSettingsResponse ClusterPutSettings(IConnection connection, ClusterPutSettingsRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			object body = RequestHelper.GetBody(request);

			return connection.Put<ClusterPutSettingsResponse>(path, parameters, body);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-update-settings.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<ClusterPutSettingsResponse> ClusterPutSettingsAsync(IConnection connection, ClusterPutSettingsRequest request)
		{
			//	TODO : async version
			return default(Task<ClusterPutSettingsResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-stats.html
		/// </summary>
		/// <param name="request">request input</param>
		public static NodesStatsResponse NodesStats(IConnection connection, NodesStatsRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<NodesStatsResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-stats.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<NodesStatsResponse> NodesStatsAsync(IConnection connection, NodesStatsRequest request)
		{
			//	TODO : async version
			return default(Task<NodesStatsResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-info.html
		/// </summary>
		/// <param name="request">request input</param>
		public static NodesInfoResponse NodesInfo(IConnection connection, NodesInfoRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<NodesInfoResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-info.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<NodesInfoResponse> NodesInfoAsync(IConnection connection, NodesInfoRequest request)
		{
			//	TODO : async version
			return default(Task<NodesInfoResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-hot-threads.html
		/// </summary>
		/// <param name="request">request input</param>
		public static NodesHotThreadsResponse NodesHotThreads(IConnection connection, NodesHotThreadsRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			return connection.Get<NodesHotThreadsResponse>(path, parameters);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-hot-threads.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<NodesHotThreadsResponse> NodesHotThreadsAsync(IConnection connection, NodesHotThreadsRequest request)
		{
			//	TODO : async version
			return default(Task<NodesHotThreadsResponse>);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-shutdown.html
		/// </summary>
		/// <param name="request">request input</param>
		public static NodesShutdownResponse NodesShutdown(IConnection connection, NodesShutdownRequest request)
		{
			IEnumerable<string> path = RequestHelper.GetPath(request);
			IDictionary<string, object> parameters = RequestHelper.GetParameters(request);
			object body = RequestHelper.GetBody(request);

			return connection.Post<NodesShutdownResponse>(path, parameters, body);
		}
		/// <summary>
		/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/master/cluster-nodes-shutdown.html
		/// </summary>
		/// <param name="request">request input</param>
		public static Task<NodesShutdownResponse> NodesShutdownAsync(IConnection connection, NodesShutdownRequest request)
		{
			//	TODO : async version
			return default(Task<NodesShutdownResponse>);
		}
	}
}
