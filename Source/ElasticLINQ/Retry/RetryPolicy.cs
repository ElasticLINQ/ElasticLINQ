// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Retry
{
    /// <summary>
    /// An implementation of <see cref="IRetryPolicy"/> which implements an exponential back-off strategy.
    /// </summary>
    public class RetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// Retries an operation if the operation is retryable.
        /// </summary>
        /// <param name="log">Used for logging debug and warning information.</param>
        /// <param name="initialRetryMilliseconds">The initial wait time for a retry. Subsequent retries grow exponentially. Defaults to 100ms.</param>
        /// <param name="maxAttempts">The maximum number of attempts to perform. Defaults to 10 attempts.</param>
        /// <param name="delay">The object which implements an async delay. Replaceable for testing purposes.</param>
        public RetryPolicy(ILog log, int initialRetryMilliseconds = 100, int maxAttempts = 10, Delay delay = null)
        {
            Log = log;
            InitialRetryMilliseconds = initialRetryMilliseconds;
            MaxAttempts = maxAttempts;
            Delay = delay ?? Delay.Instance;
        }

        internal Delay Delay { get; }

        internal int InitialRetryMilliseconds { get; }

        internal ILog Log { get; }

        internal int MaxAttempts { get; }

        /// <inheritdoc/>
        public async Task<TOperation> ExecuteAsync<TOperation>(
            Func<CancellationToken, Task<TOperation>> operationFunc,
            Func<TOperation, Exception, bool> shouldRetryFunc,
            Action<TOperation, Dictionary<string, object>> appendLogInfoFunc = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var retryDelay = InitialRetryMilliseconds;
            var attempt = 0;
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                Exception operationException = null;
                var operationResult = default(TOperation);
                try
                {
                    operationResult = await operationFunc(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    operationException = ex;
                }

                if (!shouldRetryFunc(operationResult, operationException))
                {
                    if (operationException != null)
                        ExceptionDispatchInfo.Capture(operationException).Throw();

                    return operationResult;
                }

                // Something failed. Attempt to retry the operation.
                var loggerInfo = new Dictionary<string, object>
                {
                    { "category", "retry" },
                    { "elapsedMilliseconds", stopwatch.ElapsedMilliseconds },
                    { "operationRetryDelayMS", retryDelay },
                    { "operationAttempt", ++attempt },
                    { "operationName", "ElasticLINQ" }
                };

                if (appendLogInfoFunc != null)
                    appendLogInfoFunc(operationResult, loggerInfo);

                if (attempt >= MaxAttempts)
                {
                    Log.Warn(operationException, loggerInfo, "The operation failed {0} times, which is the maximum allowed.", MaxAttempts);
                    throw new RetryFailedException(MaxAttempts);
                }

                Log.Info(operationException, loggerInfo, "The operation failed (attempt #{0}) and will be retried.", attempt);

                await Delay.For(retryDelay, cancellationToken).ConfigureAwait(false);
                retryDelay = retryDelay * 2;
            }
        }
    }
}
