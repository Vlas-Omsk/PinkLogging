using System;

namespace PinkLogging.Threading
{
    public abstract class DispatcherOperation
    {
        public DispatcherOperation(SendOrPostCallback sendOrPostCallback, object state)
        {
            SendOrPostCallback = sendOrPostCallback;
            State = state;
        }

        public SendOrPostCallback SendOrPostCallback { get; }
        public object State { get; }

        public abstract void Execute();
    }
}
