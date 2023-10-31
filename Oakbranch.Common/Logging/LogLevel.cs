using System;

namespace Oakbranch.Common.Logging
{
    /// <summary>
    /// Represents different levels of log severity.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs that are used for interactive investigation during development.
        /// <para>These logs should primarily contain information useful for debugging and have no long-term value.</para>
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Logs that track the general flow of the application.
        /// <para>These logs should have long-term value.</para>
        /// </summary>
        Info = 1,
        /// <summary>
        /// Logs that highlight abnormal or unexpected events in the application flow
        /// not causing a termination of operations involved.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Logs that highlight failures causing a termination of the current flow of the application.
        /// </summary>
        Error = 3
    }
}
