// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;

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