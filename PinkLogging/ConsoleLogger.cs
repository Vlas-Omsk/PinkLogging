using PinkLogging.Threading;
using System;
using System.Diagnostics;
using System.Reflection;

namespace PinkLogging
{
    public sealed class ConsoleLogger : Logger, IDisposable
    {
        private readonly Dispatcher _dispatcher = new Dispatcher("ConsoleLogger");

        ~ConsoleLogger()
        {
            Dispose();
        }

        protected override void Log(LogLevel level, StackFrame frame, string message)
        {
            var method = GetUserCodeFrame(frame).GetMethod() ?? frame.GetMethod();

            var levelName = level.ToString();
            ConsoleColor? levelColor = level switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.Yellow,
                _ => null
            };

            var header = $"[{method.DeclaringType.Name}.{method.Name} {frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()} ";
            var headerLength = header.Length + levelName.Length + 1;

            _dispatcher.Post((state) =>
            {
                var foregroundColor = Console.ForegroundColor;

                Console.Write(header);

                if (levelColor.HasValue)
                    Console.ForegroundColor = levelColor.Value;

                Console.Write(levelName.ToUpper());

                if (levelColor.HasValue)
                    Console.ForegroundColor = foregroundColor;

                Console.Write("] ");

                if (message != null)
                    Console.WriteLine(message
                        .Replace(Environment.NewLine, Environment.NewLine + new string(' ', headerLength)));
                else
                    Console.WriteLine();
            }, null);
        }

        private StackFrame GetUserCodeFrame(StackFrame frame)
        {
            StackFrame userCodeFrame;
            MethodBase method;

            for (var i = 4; ; i++)
            {
                userCodeFrame = new StackFrame(i, false);
                method = userCodeFrame.GetMethod();
                if (method == null || !method.DeclaringType.FullName.StartsWith("System"))
                    break;
            }

            return userCodeFrame;
        }

        public void Dispose()
        {
            _dispatcher.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
