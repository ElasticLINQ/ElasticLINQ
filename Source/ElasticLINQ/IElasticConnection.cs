﻿namespace ElasticLinq
{
    using System;
    using System.Threading.Tasks;
    using ElasticLinq.Logging;

    public interface IElasticConnection
    {
        TimeSpan Timeout { get; }

        string Index { get; }

        Uri Endpoint { get; }

        ElasticConnectionOptions Options { get; }

        Task<bool> Head(Uri uri, ILog log);

        Task<TResponse> Get<TResponse>(Uri uri, ILog log);

        Task<TResponse> Post<TResponse>(Uri uri, string body, ILog log);
    }
}
