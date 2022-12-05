using System;
using System.Diagnostics;

namespace PinkLogging
{
    public abstract class Logger : ILogger
    {
        public void Error(string message)
        {
            LogInternal(LogLevel.Error, message);
        }

        public void Warning(string message)
        {
            LogInternal(LogLevel.Warning, message);
        }

        public void Info(string message)
        {
            LogInternal(LogLevel.Info, message);
        }

        public void Trace(string message)
        {
            LogInternal(LogLevel.Trace, message);
        }

        public abstract void Log(LogLevel level, StackFrame frame, string message);

        private void LogInternal(LogLevel level, string message)
        {
            Log(level, new StackFrame(2, true), message);
        }
    }
}
