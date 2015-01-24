using System;
using ElasticLinq.Response.Model;

namespace ElasticLinq.Response.Materializers
{
    internal class AnyElasticMaterializer : IElasticMaterializer
    {
        /// <summary>
        /// Materialize whether at least one result exists for a given ElasticResponse.
        /// </summary>
        /// <param name="response">ElasticResponse to obtain the existence of a result.</param>
        /// <returns>The existence expressed as a boolean.  If count is 0, false.  Otherwise true</returns>
        public object Materialize(ElasticResponse response)
        {
            if (response.hits.total < 0)
                throw new ArgumentOutOfRangeException("response", "Contains a negative number of hits.");

            return response.hits.total > 0;
        }
    }
}