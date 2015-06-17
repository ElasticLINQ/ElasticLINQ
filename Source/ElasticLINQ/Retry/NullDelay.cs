// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Retry
{
    /// <summary>
    /// Delay implementation that does not actually delay.
    /// </summary>
    public class NullDelay : Delay
    {
        public static readonly new NullDelay Instance = new NullDelay();

        public override Task For(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}
