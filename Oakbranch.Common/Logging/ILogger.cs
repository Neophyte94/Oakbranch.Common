using System;

namespace Oakbranch.Common.Logging
{
    public interface ILogger
    {
        LogLevel Level { get; }

        bool IsLevelEnabled(LogLevel level);
        void Log(LogLevel level, string context, string message);
    }
}
