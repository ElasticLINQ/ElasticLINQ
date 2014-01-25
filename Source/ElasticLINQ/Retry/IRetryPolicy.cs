// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLinq.Retry
{
    public interface IRetryPolicy
    {
        /// <summary>
        /// Attempts an asynchronous operation one or more times.
        /// </summary>
        /// <typeparam name="TResult">The result type from the operation.</typeparam>
        /// <param name="operationFunc">The lambda which performs the operation once.</param>
        /// <param name="shouldRetryFunc">The lambda which inspects a result and/or exception and decides whether it should retry the result.</param>
        /// <param name="appendLogInfoFunc">The lambda which can supplement info logging for failed searches.</param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync<TResult>(
            Func<Task<TResult>> operationFunc,
            Func<TResult, Exception, bool> shouldRetryFunc,
            Action<TResult, Dictionary<string, object>> appendLogInfoFunc = null);
    }
}
