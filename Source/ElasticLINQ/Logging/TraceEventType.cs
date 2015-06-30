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
        /// <summary>
        /// Fatal error or application crash.
        /// </summary>
        Critical = 1,
        
        /// <summary>
        /// Recoverable error.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Noncritical problem.
        /// </summary>
        Warning = 4,
        
        /// <summary>
        /// Informational message.
        /// </summary>
        Information = 8,
        
        /// <summary>
        /// Debugging trace.
        /// </summary>
        Verbose = 16,
        
        /// <summary>
        /// Starting of a logical operation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)] Start = 256,
        
        /// <summary>
        /// Stopping of a logical operation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)] Stop = 512,
        
        /// <summary>
        /// Suspension of a logical operation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)] Suspend = 1024,
        
        /// <summary>
        /// Resumption of a logical operation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)] Resume = 2048,
        
        /// <summary>
        /// Changing of correlation identity.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)] Transfer = 4096,
    }
}