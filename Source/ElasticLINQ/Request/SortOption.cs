// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Specifies the options desired for sorting by an individual field.
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// Create a new SortOption for the given name, order and ignore.
        /// </summary>
        /// <param name="name">Name of the field to sort by.</param>
        /// <param name="ascending">True if this field should be in ascending order; false otherwise.</param>
        /// <param name="unmappedType">What ElasticSearch should unmapped values be treated as.</param>
        public SortOption(string name, bool ascending, string unmappedType = null)
        {
            Argument.EnsureNotBlank(nameof(name), name);

            Name = name;
            Ascending = ascending;
            UnmappedType = unmappedType;
        }

        /// <summary>
        /// Name of the field to be sorted.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether this field should be sorted in ascending order or not.
        /// </summary>
        public bool Ascending { get; }

        /// <summary>
        /// What ElasticSearch type should unmapped values be treated as.
        /// </summary>
        public string UnmappedType { get; }
    }
}