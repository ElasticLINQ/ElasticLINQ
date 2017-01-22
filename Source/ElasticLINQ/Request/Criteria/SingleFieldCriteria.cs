// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Base class for any criteria that maps to a single field.
    /// </summary>
    public abstract class SingleFieldCriteria : ICriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFieldCriteria"/> class.
        /// </summary>
        /// <param name="field">Field this criteria applies to.</param>
        protected SingleFieldCriteria(string field)
        {
            Argument.EnsureNotBlank(nameof(field), field);
            Field = field;
        }

        /// <summary>
        /// Field this criteria applies to.
        /// </summary>
        public string Field { get; }

        /// <inheritdoc/>
        public abstract string Name
        {
            get;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, Field);
        }
    }
}