// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.ObjectModel;
using System.Reflection;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Interface implemented by <see cref="TermCriteria"/> and <see cref="TermsCriteria"/> so that they can be
    /// treated homogeneously.
    /// </summary>
    interface ITermsCriteria : ICriteria
    {
        /// <summary>
        /// Gets the field to be searched.
        /// </summary>
        string Field { get; }

        /// <summary>
        /// Gets a value that indicates whether this criteria is an "or" style criteria.
        /// </summary>
        bool IsOrCriteria { get; }

        /// <summary>
        /// Gets the member that this criteria is searching.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        /// Gets the list of values to be searched for.
        /// </summary>
        ReadOnlyCollection<object> Values { get; }
    }
}
