using System;
using ElasticLinq.Response.Model;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a count operation by obtaining the total hits.
    /// </summary>
    internal class CountElasticMaterializer : IElasticMaterializer
    {
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