// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Async
{
    /// <summary>
    /// Defines methods to asyncronously execute queries that are described by an <see cref="T:System.Linq.IQueryable" /> object.
    /// </summary>
    public interface IAsyncQueryExecutor
    {
        /// <summary>
        /// Executes the query represented by a specified expression tree asyncronously.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>The task that returns the value that results from executing the specified query.</returns>
        Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes the strongly-typed query represented by a specified expression tree asyncronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <param name="cancellationToken">The optional token to monitor for cancellation requests.</param>
        /// <returns>The task that returns the value that results from executing the specified query.</returns>
        Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default(CancellationToken));
    }
}