// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;
using System.Diagnostics;

namespace ElasticLinq.Request.Facets
{
    /// <summary>
    /// Represents a terms_stats facet.
    /// Terms_stats facets return all statistical information for
    /// a given field broken down by a term. 
    /// </summary>
    /// <remarks>Mapped to .GroupBy(a => a.Term).Select(a => a.Sum(b => b.Field))</remarks>
    [DebuggerDisplay("TermsStatsFacet \"{Key,nq}.{Value,nq}\"")]
    class TermsStatsFacet : IOrderableFacet
    {
        public TermsStatsFacet(string name, string key, string value, int? size)
            : this(name, null, key, value)
        {
            Size = size;
        }

        public TermsStatsFacet(string name, ICriteria criteria, string key, string value)
        {
            Argument.EnsureNotBlank(nameof(name), name);
            Argument.EnsureNotBlank(nameof(key), key);
            Argument.EnsureNotBlank(nameof(value), value);

            Name = name;
            Filter = criteria;
            Key = key;
            Value = value;
        }

        public string Type => "terms_stats";
        public string Name { get; }

        public ICriteria Filter { get; }

        public string Key { get; }

        public string Value { get; }

        public int? Size { get; }
    }
}