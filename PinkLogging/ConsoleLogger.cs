using System;
using System.Diagnostics;
using System.Reflection;

namespace PinkLogging
{
    public sealed class ConsoleLogger : Logger
    {
        protected override void Log(LogLevel level, StackFrame frame, string message)
        {
            StackFrame userCodeFrame;
            MethodBase method;

            for (var i = 3; ; i++)
            {
                userCodeFrame = new StackFrame(i, false);
                method = userCodeFrame.GetMethod();
                if (!method.DeclaringType.FullName.StartsWith("System"))
                    break;
            }

            var levelName = level.ToString();
            var position = $"{frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()} ";
            var header = $"[{method.DeclaringType.Name}.{method.Name} {position}{levelName.ToUpper()}] ";

            Console.Write(header);
            if (message != null)
                Console.Write(message
                    .Replace(Environment.NewLine, Environment.NewLine + new string(' ', header.Length)));
            Console.WriteLine();
        }
    }
}
