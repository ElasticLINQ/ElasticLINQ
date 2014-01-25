// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal static class LogExtensions
{
    public static void Debug(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string message, params object[] args)
    {
        log.Log(TraceEventType.Verbose, ex, additionalInfo, message, args);
    }

    public static void Error(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string message, params object[] args)
    {
        log.Log(TraceEventType.Error, ex, additionalInfo, message, args);
    }

    public static void Fatal(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string message, params object[] args)
    {
        log.Log(TraceEventType.Critical, ex, additionalInfo, message, args);
    }

    public static void Info(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string message, params object[] args)
    {
        log.Log(TraceEventType.Information, ex, additionalInfo, message, args);
    }

    public static void Warn(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string message, params object[] args)
    {
        log.Log(TraceEventType.Warning, ex, additionalInfo, message, args);
    }
}
