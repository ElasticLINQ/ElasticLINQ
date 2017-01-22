// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that will match all documents.
    /// </summary>
    [DebuggerDisplay("match_all")]
    class MatchAllCriteria : ICriteria
    {
        /// <summary>
        /// Get the single instance of the <see cref="MatchAllCriteria"/> class.
        /// </summary>
        public static readonly MatchAllCriteria Instance = new MatchAllCriteria();

        MatchAllCriteria() { }

        /// <inheritdoc/>
        public string Name => "match_all";
    }
}