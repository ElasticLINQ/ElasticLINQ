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
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document (use `_all` to fetch the first document matching the ID across all types)</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields to return in the response</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>The ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		public string Parent { get; set; }
        /// <summary>
        /// <para>Specify the node or shard the operation should be performed on (default: random)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: preference</para>
        /// </summary>
		public string Preference { get; set; }
        /// <summary>
        /// <para>Specify whether to perform the operation in realtime or search mode</para>
		/// <para>Type: parameter</para>
        /// <para>Name: realtime</para>
        /// </summary>
		public Nullable<bool> Realtime { get; set; }
        /// <summary>
        /// <para>Refresh the shard containing the document before performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		public string Routing { get; set; }
        /// <summary>
        /// <para>True or false to return the _source field or not, or a list of fields to return</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source</para>
        /// </summary>
		public IEnumerable<string> Source { get; set; }
        /// <summary>
        /// <para>A list of fields to exclude from the returned _source field</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source_exclude</para>
        /// </summary>
		public IEnumerable<string> SourceExclude { get; set; }
        /// <summary>
        /// <para>A list of fields to extract and return from the _source field</para>
		/// <para>Type: parameter</para>
        /// <para>Name: _source_include</para>
        /// </summary>
		public IEnumerable<string> SourceInclude { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
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
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// <para>Explicit write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		public string Consistency { get; set; }
        /// <summary>
        /// <para>Explicit operation type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: op_type</para>
        /// </summary>
		public string OpType { get; set; }
        /// <summary>
        /// <para>ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		public string Routing { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit timestamp for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timestamp</para>
        /// </summary>
		public Nullable<TimeSpan> Timestamp { get; set; }
        /// <summary>
        /// <para>Expiration time for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: ttl</para>
        /// </summary>
		public Nullable<TimeSpan> Ttl { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
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
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// <para>Specific write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		public string Consistency { get; set; }
        /// <summary>
        /// <para>ID of parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		public string Routing { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
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
		public string Id { get; set; }
        /// <summary>
        /// <para>The name of the index</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Index { get; set; }
        /// <summary>
        /// <para>The type of the document</para>
		/// <para>Type: url</para>
        /// <para>Required: True</para>
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// <para>Explicit write consistency setting for the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: consistency</para>
        /// </summary>
		public string Consistency { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields to return in the response</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>The script language (default: groovy)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: lang</para>
        /// </summary>
		public string Lang { get; set; }
        /// <summary>
        /// <para>ID of the parent document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: parent</para>
        /// </summary>
		public string Parent { get; set; }
        /// <summary>
        /// <para>Refresh the index after performing the operation</para>
		/// <para>Type: parameter</para>
        /// <para>Name: refresh</para>
        /// </summary>
		public Nullable<bool> Refresh { get; set; }
        /// <summary>
        /// <para>Specific replication type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: replication</para>
        /// </summary>
		public string Replication { get; set; }
        /// <summary>
        /// <para>Specify how many times should the operation be retried when a conflict occurs (default: 0)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: retry_on_conflict</para>
        /// </summary>
		public Nullable<long> RetryOnConflict { get; set; }
        /// <summary>
        /// <para>Specific routing value</para>
		/// <para>Type: parameter</para>
        /// <para>Name: routing</para>
        /// </summary>
		public string Routing { get; set; }
        /// <summary>
        /// <para>The URL-encoded script definition (instead of using request body)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: script</para>
        /// </summary>
		public string Script { get; set; }
        /// <summary>
        /// <para>The id of a stored script</para>
		/// <para>Type: parameter</para>
        /// <para>Name: script_id</para>
        /// </summary>
		public string ScriptId { get; set; }
        /// <summary>
        /// <para>True if the script referenced in script or script_id should be called to perform inserts - defaults to false</para>
		/// <para>Type: parameter</para>
        /// <para>Name: scripted_upsert</para>
        /// </summary>
		public Nullable<bool> ScriptedUpsert { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Explicit timestamp for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timestamp</para>
        /// </summary>
		public Nullable<TimeSpan> Timestamp { get; set; }
        /// <summary>
        /// <para>Expiration time for the document</para>
		/// <para>Type: parameter</para>
        /// <para>Name: ttl</para>
        /// </summary>
		public Nullable<TimeSpan> Ttl { get; set; }
        /// <summary>
        /// <para>Explicit version number for concurrency control</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version</para>
        /// </summary>
		public Nullable<long> Version { get; set; }
        /// <summary>
        /// <para>Specific version type</para>
		/// <para>Type: parameter</para>
        /// <para>Name: version_type</para>
        /// </summary>
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
		public string Index { get; set; }
        /// <summary>
        /// <para>Specify the level of detail for returned information</para>
		/// <para>Type: parameter</para>
        /// <para>Name: level</para>
        /// </summary>
		public string Level { get; set; }
        /// <summary>
        /// <para>Return local information, do not retrieve the state from master node (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: local</para>
        /// </summary>
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
		public Nullable<TimeSpan> Timeout { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of shards is active</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_active_shards</para>
        /// </summary>
		public Nullable<long> WaitForActiveShards { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of nodes is available</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_nodes</para>
        /// </summary>
		public string WaitForNodes { get; set; }
        /// <summary>
        /// <para>Wait until the specified number of relocating shards is finished</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_relocating_shards</para>
        /// </summary>
		public Nullable<long> WaitForRelocatingShards { get; set; }
        /// <summary>
        /// <para>Wait until cluster is in a specific state</para>
		/// <para>Type: parameter</para>
        /// <para>Name: wait_for_status</para>
        /// </summary>
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
		public IEnumerable<string> Index { get; set; }
        /// <summary>
        /// <para>Limit the information returned to the specified metrics</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Return local information, do not retrieve the state from master node (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: local</para>
        /// </summary>
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Specify timeout for connection to master</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
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
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
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
		public Nullable<bool> Local { get; set; }
        /// <summary>
        /// <para>Specify timeout for connection to master</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
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
		public Nullable<bool> DryRun { get; set; }
        /// <summary>
        /// <para>Return an explanation of why the commands can or cannot be executed</para>
		/// <para>Type: parameter</para>
        /// <para>Name: explain</para>
        /// </summary>
		public Nullable<bool> Explain { get; set; }
        /// <summary>
        /// <para>Limit the information returned to the specified metrics. Defaults to all but metadata</para>
		/// <para>Type: parameter</para>
        /// <para>Name: metric</para>
        /// </summary>
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
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
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout for connection to master node</para>
		/// <para>Type: parameter</para>
        /// <para>Name: master_timeout</para>
        /// </summary>
		public Nullable<TimeSpan> MasterTimeout { get; set; }
        /// <summary>
        /// <para>Explicit operation timeout</para>
		/// <para>Type: parameter</para>
        /// <para>Name: timeout</para>
        /// </summary>
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
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Limit the information returned for `indices` metric to the specific index metrics. Isn't used if `indices` (or `all`) metric isn't specified.</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		public IEnumerable<string> IndexMetric { get; set; }
        /// <summary>
        /// <para>A comma-separated list of node IDs or names to limit the returned information; use `_local` to return information from the node you're connecting to, leave empty to get information from all nodes</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` and `suggest` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: completion_fields</para>
        /// </summary>
		public IEnumerable<string> CompletionFields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fielddata_fields</para>
        /// </summary>
		public IEnumerable<string> FielddataFields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of fields for `fielddata` and `completion` index metric (supports wildcards)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: fields</para>
        /// </summary>
		public IEnumerable<string> Fields { get; set; }
        /// <summary>
        /// <para>A comma-separated list of search groups for `search` index metric</para>
		/// <para>Type: parameter</para>
        /// <para>Name: groups</para>
        /// </summary>
		public Nullable<bool> Groups { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
		public Nullable<bool> Human { get; set; }
        /// <summary>
        /// <para>Return indices stats aggregated at node, index or shard level</para>
		/// <para>Type: parameter</para>
        /// <para>Name: level</para>
        /// </summary>
		public string Level { get; set; }
        /// <summary>
        /// <para>A comma-separated list of document types for the `indexing` index metric</para>
		/// <para>Type: parameter</para>
        /// <para>Name: types</para>
        /// </summary>
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
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>A comma-separated list of metrics you wish returned. Leave empty to return all.</para>
		/// <para>Type: url</para>
        /// <para>Required: False</para>
        /// </summary>
		public IEnumerable<string> Metric { get; set; }
        /// <summary>
        /// <para>Return settings in flat format (default: false)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: flat_settings</para>
        /// </summary>
		public Nullable<bool> FlatSettings { get; set; }
        /// <summary>
        /// <para>Whether to return time and byte values in human-readable format.</para>
		/// <para>Type: parameter</para>
        /// <para>Name: human</para>
        /// </summary>
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
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>The interval for the second sampling of threads</para>
		/// <para>Type: parameter</para>
        /// <para>Name: interval</para>
        /// </summary>
		public Nullable<TimeSpan> Interval { get; set; }
        /// <summary>
        /// <para>Number of samples of thread stacktrace (default: 10)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: snapshots</para>
        /// </summary>
		public Nullable<long> Snapshots { get; set; }
        /// <summary>
        /// <para>Specify the number of threads to provide information for (default: 3)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: threads</para>
        /// </summary>
		public Nullable<long> Threads { get; set; }
        /// <summary>
        /// <para>The type to sample (default: cpu)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: type</para>
        /// </summary>
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
		public IEnumerable<string> NodeId { get; set; }
        /// <summary>
        /// <para>Set the delay for the operation (default: 1s)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: delay</para>
        /// </summary>
		public Nullable<TimeSpan> Delay { get; set; }
        /// <summary>
        /// <para>Exit the JVM as well (default: true)</para>
		/// <para>Type: parameter</para>
        /// <para>Name: exit</para>
        /// </summary>
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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
			IEnumerable<string> path = Routing.GetPath(request);
			IDictionary<string, object> parameters = Routing.GetParameters(request);
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

namespace ElasticApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Requests;

	public static class Routing
	{

		private static readonly List<Func<GetRequest, string>[]> GetPaths = new List<Func<GetRequest, string>[]>
		{
			new Func<GetRequest, string>[]
			{
				x => RequestHelper.Segment(x.Index),
				x => RequestHelper.Segment(x.Type),
				x => RequestHelper.Segment(x.Id),
			},
		};
		
		public static IEnumerable<string> GetPath(GetRequest request)
		{
			var path = GetPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(GetRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Fields);

				if (value != null)
				{
					parameters.Add("fields", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Parent);

				if (value != null)
				{
					parameters.Add("parent", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Preference);

				if (value != null)
				{
					parameters.Add("preference", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Realtime);

				if (value != null)
				{
					parameters.Add("realtime", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Refresh);

				if (value != null)
				{
					parameters.Add("refresh", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Routing);

				if (value != null)
				{
					parameters.Add("routing", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Source);

				if (value != null)
				{
					parameters.Add("_source", value);
				}
			}
			{
				object value = RequestHelper.Param(request.SourceExclude);

				if (value != null)
				{
					parameters.Add("_source_exclude", value);
				}
			}
			{
				object value = RequestHelper.Param(request.SourceInclude);

				if (value != null)
				{
					parameters.Add("_source_include", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Version);

				if (value != null)
				{
					parameters.Add("version", value);
				}
			}
			{
				object value = RequestHelper.Param(request.VersionType);

				if (value != null)
				{
					parameters.Add("version_type", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<IndexRequest, string>[]> IndexPaths = new List<Func<IndexRequest, string>[]>
		{
			new Func<IndexRequest, string>[]
			{
				x => RequestHelper.Segment(x.Index),
				x => RequestHelper.Segment(x.Type),
				x => RequestHelper.Segment(x.Id),
			},
			new Func<IndexRequest, string>[]
			{
				x => RequestHelper.Segment(x.Index),
				x => RequestHelper.Segment(x.Type),
			},
		};
		
		public static IEnumerable<string> GetPath(IndexRequest request)
		{
			var path = IndexPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(IndexRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Consistency);

				if (value != null)
				{
					parameters.Add("consistency", value);
				}
			}
			{
				object value = RequestHelper.Param(request.OpType);

				if (value != null)
				{
					parameters.Add("op_type", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Parent);

				if (value != null)
				{
					parameters.Add("parent", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Refresh);

				if (value != null)
				{
					parameters.Add("refresh", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Replication);

				if (value != null)
				{
					parameters.Add("replication", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Routing);

				if (value != null)
				{
					parameters.Add("routing", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timestamp);

				if (value != null)
				{
					parameters.Add("timestamp", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Ttl);

				if (value != null)
				{
					parameters.Add("ttl", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Version);

				if (value != null)
				{
					parameters.Add("version", value);
				}
			}
			{
				object value = RequestHelper.Param(request.VersionType);

				if (value != null)
				{
					parameters.Add("version_type", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<DeleteRequest, string>[]> DeletePaths = new List<Func<DeleteRequest, string>[]>
		{
			new Func<DeleteRequest, string>[]
			{
				x => RequestHelper.Segment(x.Index),
				x => RequestHelper.Segment(x.Type),
				x => RequestHelper.Segment(x.Id),
			},
		};
		
		public static IEnumerable<string> GetPath(DeleteRequest request)
		{
			var path = DeletePaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(DeleteRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Consistency);

				if (value != null)
				{
					parameters.Add("consistency", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Parent);

				if (value != null)
				{
					parameters.Add("parent", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Refresh);

				if (value != null)
				{
					parameters.Add("refresh", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Replication);

				if (value != null)
				{
					parameters.Add("replication", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Routing);

				if (value != null)
				{
					parameters.Add("routing", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Version);

				if (value != null)
				{
					parameters.Add("version", value);
				}
			}
			{
				object value = RequestHelper.Param(request.VersionType);

				if (value != null)
				{
					parameters.Add("version_type", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<UpdateRequest, string>[]> UpdatePaths = new List<Func<UpdateRequest, string>[]>
		{
			new Func<UpdateRequest, string>[]
			{
				x => RequestHelper.Segment(x.Index),
				x => RequestHelper.Segment(x.Type),
				x => RequestHelper.Segment(x.Id),
				x => "_update",
			},
		};
		
		public static IEnumerable<string> GetPath(UpdateRequest request)
		{
			var path = UpdatePaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(UpdateRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Consistency);

				if (value != null)
				{
					parameters.Add("consistency", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Fields);

				if (value != null)
				{
					parameters.Add("fields", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Lang);

				if (value != null)
				{
					parameters.Add("lang", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Parent);

				if (value != null)
				{
					parameters.Add("parent", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Refresh);

				if (value != null)
				{
					parameters.Add("refresh", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Replication);

				if (value != null)
				{
					parameters.Add("replication", value);
				}
			}
			{
				object value = RequestHelper.Param(request.RetryOnConflict);

				if (value != null)
				{
					parameters.Add("retry_on_conflict", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Routing);

				if (value != null)
				{
					parameters.Add("routing", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Script);

				if (value != null)
				{
					parameters.Add("script", value);
				}
			}
			{
				object value = RequestHelper.Param(request.ScriptId);

				if (value != null)
				{
					parameters.Add("script_id", value);
				}
			}
			{
				object value = RequestHelper.Param(request.ScriptedUpsert);

				if (value != null)
				{
					parameters.Add("scripted_upsert", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timestamp);

				if (value != null)
				{
					parameters.Add("timestamp", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Ttl);

				if (value != null)
				{
					parameters.Add("ttl", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Version);

				if (value != null)
				{
					parameters.Add("version", value);
				}
			}
			{
				object value = RequestHelper.Param(request.VersionType);

				if (value != null)
				{
					parameters.Add("version_type", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterHealthRequest, string>[]> ClusterHealthPaths = new List<Func<ClusterHealthRequest, string>[]>
		{
			new Func<ClusterHealthRequest, string>[]
			{
				x => "_cluster",
				x => "health",
			},
			new Func<ClusterHealthRequest, string>[]
			{
				x => "_cluster",
				x => "health",
				x => RequestHelper.Segment(x.Index),
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterHealthRequest request)
		{
			var path = ClusterHealthPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterHealthRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Level);

				if (value != null)
				{
					parameters.Add("level", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Local);

				if (value != null)
				{
					parameters.Add("local", value);
				}
			}
			{
				object value = RequestHelper.Param(request.MasterTimeout);

				if (value != null)
				{
					parameters.Add("master_timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.WaitForActiveShards);

				if (value != null)
				{
					parameters.Add("wait_for_active_shards", value);
				}
			}
			{
				object value = RequestHelper.Param(request.WaitForNodes);

				if (value != null)
				{
					parameters.Add("wait_for_nodes", value);
				}
			}
			{
				object value = RequestHelper.Param(request.WaitForRelocatingShards);

				if (value != null)
				{
					parameters.Add("wait_for_relocating_shards", value);
				}
			}
			{
				object value = RequestHelper.Param(request.WaitForStatus);

				if (value != null)
				{
					parameters.Add("wait_for_status", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterStateRequest, string>[]> ClusterStatePaths = new List<Func<ClusterStateRequest, string>[]>
		{
			new Func<ClusterStateRequest, string>[]
			{
				x => "_cluster",
				x => "state",
			},
			new Func<ClusterStateRequest, string>[]
			{
				x => "_cluster",
				x => "state",
				x => RequestHelper.Segment(x.Metric),
			},
			new Func<ClusterStateRequest, string>[]
			{
				x => "_cluster",
				x => "state",
				x => RequestHelper.Segment(x.Metric),
				x => RequestHelper.Segment(x.Index),
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterStateRequest request)
		{
			var path = ClusterStatePaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterStateRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Local);

				if (value != null)
				{
					parameters.Add("local", value);
				}
			}
			{
				object value = RequestHelper.Param(request.MasterTimeout);

				if (value != null)
				{
					parameters.Add("master_timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.FlatSettings);

				if (value != null)
				{
					parameters.Add("flat_settings", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterStatsRequest, string>[]> ClusterStatsPaths = new List<Func<ClusterStatsRequest, string>[]>
		{
			new Func<ClusterStatsRequest, string>[]
			{
				x => "_cluster",
				x => "stats",
			},
			new Func<ClusterStatsRequest, string>[]
			{
				x => "_cluster",
				x => "stats",
				x => "nodes",
				x => RequestHelper.Segment(x.NodeId),
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterStatsRequest request)
		{
			var path = ClusterStatsPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterStatsRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.FlatSettings);

				if (value != null)
				{
					parameters.Add("flat_settings", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Human);

				if (value != null)
				{
					parameters.Add("human", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterPendingTasksRequest, string>[]> ClusterPendingTasksPaths = new List<Func<ClusterPendingTasksRequest, string>[]>
		{
			new Func<ClusterPendingTasksRequest, string>[]
			{
				x => "_cluster",
				x => "pending_tasks",
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterPendingTasksRequest request)
		{
			var path = ClusterPendingTasksPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterPendingTasksRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Local);

				if (value != null)
				{
					parameters.Add("local", value);
				}
			}
			{
				object value = RequestHelper.Param(request.MasterTimeout);

				if (value != null)
				{
					parameters.Add("master_timeout", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterRerouteRequest, string>[]> ClusterReroutePaths = new List<Func<ClusterRerouteRequest, string>[]>
		{
			new Func<ClusterRerouteRequest, string>[]
			{
				x => "_cluster",
				x => "reroute",
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterRerouteRequest request)
		{
			var path = ClusterReroutePaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterRerouteRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.DryRun);

				if (value != null)
				{
					parameters.Add("dry_run", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Explain);

				if (value != null)
				{
					parameters.Add("explain", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Metric);

				if (value != null)
				{
					parameters.Add("metric", value);
				}
			}
			{
				object value = RequestHelper.Param(request.MasterTimeout);

				if (value != null)
				{
					parameters.Add("master_timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterGetSettingsRequest, string>[]> ClusterGetSettingsPaths = new List<Func<ClusterGetSettingsRequest, string>[]>
		{
			new Func<ClusterGetSettingsRequest, string>[]
			{
				x => "_cluster",
				x => "settings",
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterGetSettingsRequest request)
		{
			var path = ClusterGetSettingsPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterGetSettingsRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.FlatSettings);

				if (value != null)
				{
					parameters.Add("flat_settings", value);
				}
			}
			{
				object value = RequestHelper.Param(request.MasterTimeout);

				if (value != null)
				{
					parameters.Add("master_timeout", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Timeout);

				if (value != null)
				{
					parameters.Add("timeout", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<ClusterPutSettingsRequest, string>[]> ClusterPutSettingsPaths = new List<Func<ClusterPutSettingsRequest, string>[]>
		{
			new Func<ClusterPutSettingsRequest, string>[]
			{
				x => "_cluster",
				x => "settings",
			},
		};
		
		public static IEnumerable<string> GetPath(ClusterPutSettingsRequest request)
		{
			var path = ClusterPutSettingsPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(ClusterPutSettingsRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.FlatSettings);

				if (value != null)
				{
					parameters.Add("flat_settings", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<NodesStatsRequest, string>[]> NodesStatsPaths = new List<Func<NodesStatsRequest, string>[]>
		{
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => "stats",
			},
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "stats",
			},
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => "stats",
				x => RequestHelper.Segment(x.Metric),
			},
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "stats",
				x => RequestHelper.Segment(x.Metric),
			},
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => "stats",
				x => RequestHelper.Segment(x.Metric),
				x => RequestHelper.Segment(x.IndexMetric),
			},
			new Func<NodesStatsRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "stats",
				x => RequestHelper.Segment(x.Metric),
				x => RequestHelper.Segment(x.IndexMetric),
			},
		};
		
		public static IEnumerable<string> GetPath(NodesStatsRequest request)
		{
			var path = NodesStatsPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(NodesStatsRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.CompletionFields);

				if (value != null)
				{
					parameters.Add("completion_fields", value);
				}
			}
			{
				object value = RequestHelper.Param(request.FielddataFields);

				if (value != null)
				{
					parameters.Add("fielddata_fields", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Fields);

				if (value != null)
				{
					parameters.Add("fields", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Groups);

				if (value != null)
				{
					parameters.Add("groups", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Human);

				if (value != null)
				{
					parameters.Add("human", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Level);

				if (value != null)
				{
					parameters.Add("level", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Types);

				if (value != null)
				{
					parameters.Add("types", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<NodesInfoRequest, string>[]> NodesInfoPaths = new List<Func<NodesInfoRequest, string>[]>
		{
			new Func<NodesInfoRequest, string>[]
			{
				x => "_nodes",
			},
			new Func<NodesInfoRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
			},
			new Func<NodesInfoRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.Metric),
			},
			new Func<NodesInfoRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => RequestHelper.Segment(x.Metric),
			},
		};
		
		public static IEnumerable<string> GetPath(NodesInfoRequest request)
		{
			var path = NodesInfoPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(NodesInfoRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.FlatSettings);

				if (value != null)
				{
					parameters.Add("flat_settings", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Human);

				if (value != null)
				{
					parameters.Add("human", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<NodesHotThreadsRequest, string>[]> NodesHotThreadsPaths = new List<Func<NodesHotThreadsRequest, string>[]>
		{
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => "hotthreads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => "hot_threads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "hotthreads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "hot_threads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_nodes",
				x => "hotthreads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_nodes",
				x => "hot_threads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "hotthreads",
			},
			new Func<NodesHotThreadsRequest, string>[]
			{
				x => "_nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "hot_threads",
			},
		};
		
		public static IEnumerable<string> GetPath(NodesHotThreadsRequest request)
		{
			var path = NodesHotThreadsPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(NodesHotThreadsRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Interval);

				if (value != null)
				{
					parameters.Add("interval", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Snapshots);

				if (value != null)
				{
					parameters.Add("snapshots", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Threads);

				if (value != null)
				{
					parameters.Add("threads", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Type);

				if (value != null)
				{
					parameters.Add("type", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}

		private static readonly List<Func<NodesShutdownRequest, string>[]> NodesShutdownPaths = new List<Func<NodesShutdownRequest, string>[]>
		{
			new Func<NodesShutdownRequest, string>[]
			{
				x => "_shutdown",
			},
			new Func<NodesShutdownRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => "_shutdown",
			},
			new Func<NodesShutdownRequest, string>[]
			{
				x => "_cluster",
				x => "nodes",
				x => RequestHelper.Segment(x.NodeId),
				x => "_shutdown",
			},
		};
		
		public static IEnumerable<string> GetPath(NodesShutdownRequest request)
		{
			var path = NodesShutdownPaths.First();

			return path.Select(x => x(request));
		}

		public static IDictionary<string, object> GetParameters(NodesShutdownRequest request)
		{
			var parameters = new Dictionary<string, object>();
			{
				object value = RequestHelper.Param(request.Delay);

				if (value != null)
				{
					parameters.Add("delay", value);
				}
			}
			{
				object value = RequestHelper.Param(request.Exit);

				if (value != null)
				{
					parameters.Add("exit", value);
				}
			}
			RequestHelper.AddCommonParameters(parameters);

			return parameters;
		}
	}
}
