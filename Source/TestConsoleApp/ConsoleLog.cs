using ElasticLinq.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TestConsoleApp
{
    internal class ConsoleLog : ILog
    {
        readonly Stopwatch stopwatch = Stopwatch.StartNew();
        readonly object lockObject = new object();

        public void Log(TraceEventType type, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
        {
            lock (lockObject)
            {
                var fgColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;

                var message = (args == null || args.Length == 0) ? messageFormat : String.Format(messageFormat, args);
                Console.WriteLine("[{0} {1}] {2}", stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"), type, message);

                if (additionalInfo != null && additionalInfo.Count > 0)
                {
                    Console.WriteLine("Additional Info:");
                    var keyLength = additionalInfo.Keys.Max(k => k.Length);

                    foreach (var kvp in additionalInfo)
                        Console.WriteLine("  {0} = {1}", kvp.Key.PadRight(keyLength), kvp.Value);
                }

                while (ex != null)
                {
                    Console.WriteLine(ex);
                    ex = ex.InnerException;
                    if (ex != null)
                        Console.WriteLine("Inner exception:");
                }

                Console.ForegroundColor = fgColor;
            }
        }
    }
}
