// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq
{
    using System;
    using System.Threading.Tasks;
    using ElasticLinq.Logging;
    using ElasticLinq.Path;

    public interface IElasticConnection
    {
        TimeSpan Timeout { get; }

        string Index { get; }

        Uri Endpoint { get; }

        ElasticConnectionOptions Options { get; }

        Task<bool> Head(ElasticPath path, ILog log);

        Task<TResponse> Get<TResponse>(Uri uri, ILog log);

        Task<TResponse> Post<TResponse>(Uri uri, string body, ILog log);
    }
}
