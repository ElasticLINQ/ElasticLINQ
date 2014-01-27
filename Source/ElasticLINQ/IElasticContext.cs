// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq;

namespace ElasticLinq
{
    /// <summary>
    /// Represents a unit of work in ElasticLINQ.
    /// </summary>
    public interface IElasticContext
    {
        /// <summary>
        /// Gets a query that can search for documents of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The document type.</typeparam>
        /// <returns>The query that can search for documents of the given type.</returns>
        IQueryable<T> Query<T>();
    }
}
