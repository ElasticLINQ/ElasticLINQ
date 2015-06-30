// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Which mode a terms criteria should be executed in.
    /// </summary>
    /// <remarks>
    /// See the Elasticsearch documentation for more details on how each term
    /// behaves differently.
    /// </remarks>
    public enum TermsExecutionMode
    {
        /// <summary>
        /// Default mode using full iteration, caching and bit-set matching.
        /// </summary>
        plain,

        /// <summary>
        /// Use the fielddata cache to compare terms but does not cache itself.
        /// </summary>
        fielddata,

        /// <summary>
        /// Expand each term out into its own filter and wrap it in a bool filter.
        /// </summary>
        @bool,

        /// <summary>
        /// Expand each term out into its own filter and wrap it in an and filter.
        /// </summary>
        and,

        /// <summary>
        /// Expand each term out into its own filter and wrap it in an or filter.
        /// </summary>
        or
    }
}
