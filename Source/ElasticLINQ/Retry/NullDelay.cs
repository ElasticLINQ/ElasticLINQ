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
        /// <summary>
        /// Obtain a shared safe instance of the <see cref="NullDelay" />
        /// </summary>
        public new static readonly NullDelay Instance = new NullDelay();

        /// <summary>
        /// Obtain a task that will not delay.
        /// </summary>
        /// <param name="milliseconds">This parameter is ignored.</param>
        /// <param name="cancellationToken">This parameter is ignored.</param>
        /// <returns>Task that will not delay.</returns>
        public override Task For(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
    }
}
