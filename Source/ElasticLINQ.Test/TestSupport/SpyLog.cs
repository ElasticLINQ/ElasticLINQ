// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using System;
using System.Collections.Generic;

namespace ElasticLinq.Test
{
    public struct LogEntry
    {
        public TraceEventType Type;
        public Exception Exception;
        public IDictionary<string, object> AdditionalInfo;
        public string MessageFormat;
        public object[] Args;
        public string Message;
    }

    public class SpyLog : ILog
    {
        public readonly List<LogEntry> Entries = new List<LogEntry>();

        public void Log(TraceEventType type, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
        {
            Entries.Add(new LogEntry
            {
                Type = type,
                Exception = ex,
                AdditionalInfo = additionalInfo ?? new Dictionary<string, object>(),
                MessageFormat = messageFormat,
                Args = args,
                Message = args == null || args.Length == 0 ? messageFormat : string.Format(messageFormat, args)
            });
        }
    }
}