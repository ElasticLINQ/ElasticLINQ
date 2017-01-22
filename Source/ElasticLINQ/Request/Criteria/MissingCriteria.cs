// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria to select documents if they do not have a value
    /// in the specified field.
    /// </summary>
    class MissingCriteria : SingleFieldCriteria, INegatableCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingCriteria"/> class.
        /// </summary>
        /// <param name="field">Field that must be missing for this criteria to be satisfied.</param>
        public MissingCriteria(string field)
            : base(field)
        {
        }

        /// <inheritdoc/>
        public override string Name => "missing";

        /// <summary>
        /// Negate this Missing criteria by turning it into an Exists criteria.
        /// </summary>
        /// <returns>Exists criteria for this field.</returns>
        public ICriteria Negate()
        {
            return new ExistsCriteria(Field);
        }
    }
}