using System;
using ElasticLinq.Response.Model;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a count operation by obtaining the total hits.
    /// </summary>
    internal class CountElasticMaterializer : IElasticMaterializer
    {
        /// <summary>
        /// Materialize the hit count for a given ElasticResponse.
        /// </summary>
        /// <param name="response">ElasticResponse to obtain the hit values from.</param>
        /// <returns>The hit value count expressed as either an int or long depending on the size of the count.</returns>
        public object Materialize(ElasticResponse response)
        {
            if (response.hits.total < 0)
                throw new ArgumentOutOfRangeException("response", "Contains a negative number of hits.");

            if (response.hits.total <= int.MaxValue)
                return (int)response.hits.total;

            return response.hits.total;
        }
    }
}