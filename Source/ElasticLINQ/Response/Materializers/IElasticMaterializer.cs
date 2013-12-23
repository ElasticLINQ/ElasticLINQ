// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;

namespace ElasticLinq.Response.Materializers
{
    internal interface IElasticMaterializer
    {
        object Materialize(ElasticResponse response);
    }
}