using System;
using System.Diagnostics;

namespace PinkLogging
{
    public interface ILogger
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
        void Trace(string message);
        void Log(LogLevel level, StackFrame frame, string message);
    }
}