using System;

namespace PinkLogging
{
    public interface ILogger
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
        void Trace(string message);
    }
}