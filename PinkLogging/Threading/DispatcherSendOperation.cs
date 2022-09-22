using System;

namespace PinkLogging.Threading
{
    public sealed class DispatcherSendOperation : DispatcherOperation, IDisposable
    {
        public DispatcherSendOperation(SendOrPostCallback sendOrPostCallback, object state) : base(sendOrPostCallback, state)
        {
        }

        public Exception Exception { get; private set; }
        internal AutoResetEvent SyncObject { get; } = new AutoResetEvent(false);

        public bool HasException => Exception != null;

        public override void Execute()
        {
            try
            {
                SendOrPostCallback(State);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            SyncObject.Set();
        }

        public void Complete()
        {
            SyncObject.WaitOne();
            if (HasException)
                throw Exception;
        }

        public void Dispose()
        {
            SyncObject.Dispose();
        }
    }
}
