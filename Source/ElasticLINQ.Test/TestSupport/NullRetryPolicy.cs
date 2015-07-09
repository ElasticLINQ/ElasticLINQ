// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Retry;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace ElasticLinq.Test
{
    public static class NullRetryPolicy
    {
        public static readonly IRetryPolicy Instance = CreateNullRetryPolicy();

        static IRetryPolicy CreateNullRetryPolicy()
        {
            var result = Substitute.For<IRetryPolicy>();
            result
                .ExecuteAsync<ElasticResponse>(null, null)
                .ReturnsForAnyArgs(callInfo => callInfo.Arg<Func<Task<ElasticResponse>>>()());
            return result;
        }
    }
}
