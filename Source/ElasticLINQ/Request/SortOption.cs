// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Specifies the options desired for sorting by an individual field.
    /// </summary>
    class SortOption
    {
        readonly string name;
        readonly bool ascending;
        readonly bool ignoreUnmapped;

        /// <summary>
        /// Create a new SortOption for the given name, order and ignore.
        /// </summary>
        /// <param name="name">Name of the field to sort by.</param>
        /// <param name="ascending">True if this field should be in ascending order; false otherwise.</param>
        /// <param name="ignoreUnmapped">Whether unmapped fields should be ignored or not.</param>
        public SortOption(string name, bool ascending, bool ignoreUnmapped = false)
        {
            Argument.EnsureNotBlank("name", name);
            this.name = name;
            this.ascending = ascending;
            this.ignoreUnmapped = ignoreUnmapped;
        }

        /// <summary>
        /// Name of the field to be sorted.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Whether this field should be sorted in ascending order or not.
        /// </summary>
        public bool Ascending
        {
            get { return ascending; }
        }

        /// <summary>
        /// Whether documents with unmapped fields should be ignored or not.
        /// </summary>
        public bool IgnoreUnmapped
        {
            get { return ignoreUnmapped; }
        }
    }
}