using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Utility
{
    internal static class AsyncHelper
    {
        public static T RunSync<T>(Func<Task<T>> action)
        {
            if (SynchronizationContext.Current != null)
                return Task.Run(async () => await action()).GetAwaiter().GetResult();
            else
                return action().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> action)
        {
            if (SynchronizationContext.Current != null)
                Task.Run(async () => await action()).GetAwaiter().GetResult();
            else
                action().GetAwaiter().GetResult();
        }
    }
}
