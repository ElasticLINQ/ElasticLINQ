// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq
{
    using System.Linq;
    using ElasticLinq.Path;

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
        IQueryable<T> Query<T>(ElasticPath path = null);

        //bool IndexExists(ElasticIndexPath indexPath);

        //bool TypeExists(ElasticIndexPath indexPath, ElasticTypePath typePath);
    }
}
