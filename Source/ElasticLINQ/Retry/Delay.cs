// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Retry
{
    /// <summary>
    /// Implements an asynchronous delay. Replaceable for testing purposes (so unit tests don't actually wait).
    /// </summary>
    public class Delay
    {
        /// <summary>
        /// Obtain a shared safe instance of the <see cref="Delay" />
        /// </summary>
        public static readonly Delay Instance = new Delay();

        /// <summary>
        /// Obtain a task that will delay for a number of milliseconds or until the cancellation token is cancelled.
        /// </summary>
        /// <param name="milliseconds">Number of milliseconds to delay for.</param>
        /// <param name="cancellationToken">Cancellation token to watch for cancellation.</param>
        /// <returns>Task that will delay for the number of milliseconds.</returns>
        public virtual Task For(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Delay(milliseconds, cancellationToken);
        }
    }
}