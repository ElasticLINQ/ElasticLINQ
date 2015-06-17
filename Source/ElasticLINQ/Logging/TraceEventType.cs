// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.ComponentModel;

namespace ElasticLinq.Logging
{
    /// <summary>
    /// Type of log entry to write to the logging system.
    /// </summary>
    /// <remarks>
    /// Replicates that of the .NET built-in type for PCL compatibility.
    /// </remarks>
    public enum TraceEventType
    {
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
        [EditorBrowsable(EditorBrowsableState.Advanced)] Start = 256,
        [EditorBrowsable(EditorBrowsableState.Advanced)] Stop = 512,
        [EditorBrowsable(EditorBrowsableState.Advanced)] Suspend = 1024,
        [EditorBrowsable(EditorBrowsableState.Advanced)] Resume = 2048,
        [EditorBrowsable(EditorBrowsableState.Advanced)] Transfer = 4096,
    }
}