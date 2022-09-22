using System;

namespace PinkLogging.Threading
{
    public sealed class DispatcherPostOperation : DispatcherOperation
    {
        public DispatcherPostOperation(SendOrPostCallback sendOrPostCallback, object state) : base(sendOrPostCallback, state)
        {
        }

        public override void Execute()
        {
            SendOrPostCallback(State);
        }
    }
}
