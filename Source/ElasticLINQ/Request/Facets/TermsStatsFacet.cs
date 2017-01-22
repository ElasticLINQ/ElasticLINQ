// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a terms_stats facet.
    /// Terms_stats facets return all statistical information for
    /// a given field broken down by a term. 
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => a.Term).Select(a => a.Sum(b => b.Field))</remarks>
    [DebuggerDisplay("TermsStatsFacet \"{key,nq}.{value,nq}\"")]
    class TermsStatsFacet : IOrderableFacet
    {
        readonly string name;
        readonly ICriteria criteria;
        readonly string key;
        readonly string value;
        readonly int? size;

        public TermsStatsFacet(string name, string key, string value, int? size)
            : this(name, null, key, value)
        {
            this.size = size;
        }

        public TermsStatsFacet(string name, ICriteria criteria, string key, string value)
        {
            Argument.EnsureNotBlank(nameof(name), name);
            Argument.EnsureNotBlank(nameof(key), key);
            Argument.EnsureNotBlank(nameof(value), value);

            this.name = name;
            this.criteria = criteria;
            this.key = key;
            this.value = value;
        }

        public string Type => "terms_stats";
        public string Name => name;
        public ICriteria Filter => criteria;
        public string Key => key;
        public string Value => value;
        public int? Size => size;
    }
}