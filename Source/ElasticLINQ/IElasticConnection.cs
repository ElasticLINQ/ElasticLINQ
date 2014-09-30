// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq
{
    using System;
    using System.Threading.Tasks;
    using ElasticLinq.Logging;
    using ElasticLinq.Path;

    public interface IElasticConnection
    {
        ElasticConnectionOptions Options { get; }

        Task<bool> Head<TRequest>(TRequest request, ILog log);

        Task<TResponse> Get<TResponse, TRequest>(TRequest request, ILog log);

        Task<TResponse> Post<TResponse, TRequest>(TRequest request, string body, ILog log);
    }
}
