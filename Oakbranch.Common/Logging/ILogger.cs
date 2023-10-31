using System;

namespace Oakbranch.Common.Logging
{
    /// <summary>
    /// Represents an abstraction for performing logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the current log level.
        /// </summary>
        LogLevel Level { get; }

        /// <summary>
        /// Determines whether the given log level is enabled.
        /// </summary>
        /// <param name="level">The log level to check.</param>
        /// <returns><see langword="true"/> if the specified log level is enabled; otherwise, <see langword="false"/>.</returns>
        bool IsLevelEnabled(LogLevel level);
        /// <summary>
        /// Writes a log message at the specified log level.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="context">The context or category of the message.</param>
        /// <param name="message">The message to be logged.</param>
        void Log(LogLevel level, string context, string message);
    }
}