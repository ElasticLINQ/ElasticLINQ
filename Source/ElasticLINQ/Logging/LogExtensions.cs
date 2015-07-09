// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using System;
using System.Collections.Generic;

/// <summary>
/// Various extension methods to make logging to ILog more fluent.
/// </summary>
static class LogExtensions
{
    /// <summary>
    /// Logs a debug message to the logging system.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/>) to receive the message.</param>
    /// <param name="ex">The exception (optional).</param>
    /// <param name="additionalInfo">Additional information to be logged (optional).</param>
    /// <param name="messageFormat">The message (will be formatted, if <paramref name="args"/> is not null/empty; otherwise,
    /// should be sent directly to the logging system).</param>
    /// <param name="args">The arguments for <paramref name="messageFormat"/> (optional).</param>
    public static void Debug(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
    {
        log.Log(TraceEventType.Verbose, ex, additionalInfo, messageFormat, args);
    }

    /// <summary>
    /// Logs an error message to the logging system.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/>) to receive the message.</param>
    /// <param name="ex">The exception (optional).</param>
    /// <param name="additionalInfo">Additional information to be logged (optional).</param>
    /// <param name="messageFormat">The message (will be formatted, if <paramref name="args"/> is not null/empty; otherwise,
    /// should be sent directly to the logging system).</param>
    /// <param name="args">The arguments for <paramref name="messageFormat"/> (optional).</param>
    public static void Error(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
    {
        log.Log(TraceEventType.Error, ex, additionalInfo, messageFormat, args);
    }

    /// <summary>
    /// Logs a fatal message to the logging system.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/>) to receive the message.</param>
    /// <param name="ex">The exception (optional).</param>
    /// <param name="additionalInfo">Additional information to be logged (optional).</param>
    /// <param name="messageFormat">The message (will be formatted, if <paramref name="args"/> is not null/empty; otherwise,
    /// should be sent directly to the logging system).</param>
    /// <param name="args">The arguments for <paramref name="messageFormat"/> (optional).</param>
    public static void Fatal(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
    {
        log.Log(TraceEventType.Critical, ex, additionalInfo, messageFormat, args);
    }

    /// <summary>
    /// Logs an information message to the logging system.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/>) to receive the message.</param>
    /// <param name="ex">The exception (optional).</param>
    /// <param name="additionalInfo">Additional information to be logged (optional).</param>
    /// <param name="messageFormat">The message (will be formatted, if <paramref name="args"/> is not null/empty; otherwise,
    /// should be sent directly to the logging system).</param>
    /// <param name="args">The arguments for <paramref name="messageFormat"/> (optional).</param>
    public static void Info(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
    {
        log.Log(TraceEventType.Information, ex, additionalInfo, messageFormat, args);
    }

    /// <summary>
    /// Logs a warning message to the logging system.
    /// </summary>
    /// <param name="log">The <see cref="ILog"/>) to receive the message.</param>
    /// <param name="ex">The exception (optional).</param>
    /// <param name="additionalInfo">Additional information to be logged (optional).</param>
    /// <param name="messageFormat">The message (will be formatted, if <paramref name="args"/> is not null/empty; otherwise,
    /// should be sent directly to the logging system).</param>
    /// <param name="args">The arguments for <paramref name="messageFormat"/> (optional).</param>
    public static void Warn(this ILog log, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
    {
        log.Log(TraceEventType.Warning, ex, additionalInfo, messageFormat, args);
    }
}
