using ElasticLinq.Response.Model;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes total count
    /// </summary>
    internal class CountElasticMaterializer : IElasticMaterializer
    {
        public object Materialize(ElasticResponse response)
        {
            return (int)response.hits.total;
        }
    }
}