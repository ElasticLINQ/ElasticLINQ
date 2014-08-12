// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;

namespace ElasticLinq.Response.Model
{
    /// <summary>
    /// Shard statistics response from Elasticsearch.
    /// </summary>
    [DebuggerDisplay("{failed} failed, {successful} success")]
    public class ShardStatistics
    {
        public int total;
        public int successful;
        public int failed;
    }
}