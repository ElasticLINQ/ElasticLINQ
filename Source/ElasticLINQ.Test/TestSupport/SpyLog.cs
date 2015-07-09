// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Test
{
    public class SpyLog : ILog
    {
        public readonly List<string> Messages = new List<string>();

        public void Log(TraceEventType type, Exception ex, IDictionary<string, object> additionalInfo, string messageFormat, params object[] args)
        {
            string messageText = args == null || args.Length == 0 ? messageFormat : string.Format(messageFormat, args);
            string spiedMessage = string.Format("[{0}] {1}", type.ToString().ToUpperInvariant(), messageText);

            for (; ex != null; ex = ex.InnerException)
                spiedMessage += string.Format("\r\nEXCEPTION:\r\n{0}", ex);

            if (additionalInfo != null && additionalInfo.Count != 0)
                spiedMessage += string.Format("\r\nADDITIONAL INFO: {0}", string.Join(", ", additionalInfo.Select(kvp => string.Format("{0} = {1}", kvp.Key, kvp.Value))));

            Messages.Add(spiedMessage);
        }
    }
}