using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ElasticLinq.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILog"/> which does no logging.
    /// </summary>
    public sealed class NullLog : ILog
    {
        /// <summary>
        /// Gets the singleton <see cref="NullLog"/> instance.
        /// </summary>
        public static readonly NullLog Instance = new NullLog();

        NullLog() { }

        /// <inheritdoc/>
        public void Log(TraceEventType type, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, object[] args)
        {
        }
    }
}
