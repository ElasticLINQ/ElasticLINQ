// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes true or false depending on whether any results matched the query or not.
    /// </summary>
    class AnyElasticMaterializer : IElasticMaterializer
    {
        /// <summary>
        /// Determine whether at a given query response contains any hits.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to check for emptiness.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public object Materialize(ElasticResponse response)
        {
            if (response.hits.total < 0)
                throw new ArgumentOutOfRangeException(nameof(response), "Contains a negative number of hits.");

            return response.hits.total > 0;
        }
    }
}