// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a specific field needs to start with a specific prefix.
    /// </summary>
    /// <remarks>
    /// This will only work on fields within Elasticsearch that are not analyzed as otherwise the
    /// keyword tokenizer will have removed any concept of a prefix.
    /// </remarks>
    public class PrefixCriteria : SingleFieldCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixCriteria"/> class.
        /// </summary>
        /// <param name="field">Field to check the prefix of.</param>
        /// <param name="prefix">Prefix to check within this field.</param>
        public PrefixCriteria(string field, string prefix)
            : base(field)
        {
            Prefix = prefix;
        }

        /// <summary>
        /// Prefix to check the field begins with.
        /// </summary>
        public string Prefix { get; }

        /// <inheritdoc/>
        public override string Name
        {
            get { return "prefix"; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}\"{1}\"", base.ToString(), Prefix);
        }
    }
}