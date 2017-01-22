// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that inverts the logic of criteria it contains.
    /// </summary>
    class NotCriteria : ICriteria, INegatableCriteria
    {
        /// <summary>
        /// Create a negated version of the criteria supplied.
        /// </summary>
        /// <param name="criteria"><see cref="ICriteria"/> to be negated.</param>
        /// <returns><see cref="ICriteria"/> that is a negated version of the criteria supplied.</returns>
        /// <remarks>
        /// If the criteria supplied supports <see cref="INegatableCriteria" /> then it will be asked
        /// to provide its own negation, e.g. <see cref="MissingCriteria"/> becomes <see cref="ExistsCriteria"/>
        /// otherwise it will be wrapped in a <see cref="NotCriteria"/>.
        /// </remarks>
        public static ICriteria Create(ICriteria criteria)
        {
            Argument.EnsureNotNull(nameof(criteria), criteria);

            // Allow some criteria to provide their own negation instead
            return criteria is INegatableCriteria
                ? ((INegatableCriteria) criteria).Negate()
                : new NotCriteria(criteria);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotCriteria"/> class.
        /// </summary>
        /// <param name="criteria">Criteria to be negated.</param>
        /// <remarks>
        /// Consider using <see cref="NotCriteria.Create"/> instead as there may be a simpler
        /// representation of the criteria if it supports negation.
        /// </remarks>
        NotCriteria(ICriteria criteria)
        {
            Criteria = criteria;
        }

        /// <inheritdoc/>
        public string Name => "not";

        /// <summary>
        /// <see cref="ICriteria" /> that is being negated.
        /// </summary>
        public ICriteria Criteria { get; }

        /// <summary>
        /// Negate this <see cref="NotCriteria"/> by returning the criteria it is wrapping.
        /// </summary>
        /// <returns>Inner criteria no longer wrapped with Not.</returns>
        public ICriteria Negate()
        {
            return Criteria;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "not " + Criteria;
        }
    }
}