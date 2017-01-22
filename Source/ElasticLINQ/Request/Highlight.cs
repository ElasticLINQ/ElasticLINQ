// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticLinq.Request
{
    /// <summary>
    /// Specifies highlighting of search results for one or more fields.
    /// </summary>
    public class Highlight
    {
        readonly List<string> fields = new List<string>();

        internal void AddFields(params string[] newFields)
        {
            fields.AddRange(newFields);
        }

        /// <summary>
        /// The string to start the highlight of each word.
        /// </summary>
        /// <remarks>
        /// This is typically set to an opening HTML tag, hence the name.
        /// </remarks>
        public string PreTag { get; set; }

        /// <summary>
        /// The string to end the highlight of each word.
        /// </summary>
        /// <remarks>
        /// This is typically set to an closing HTML tag, hence the name.
        /// </remarks>
        public string PostTag { get; set; }

        /// <summary>
        /// The fields highlighted by this request.
        /// </summary>
        public ReadOnlyCollection<string> Fields => new ReadOnlyCollection<string>(fields);
    }
}