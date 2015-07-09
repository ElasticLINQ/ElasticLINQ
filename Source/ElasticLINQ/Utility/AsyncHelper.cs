// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Utility
{
    static class AsyncHelper
    {
        public static T RunSync<T>(Func<Task<T>> action)
        {
            return SynchronizationContext.Current == null
                ? action().GetAwaiter().GetResult()
                : Task.Run(async () => await action()).GetAwaiter().GetResult();
        }
    }
}