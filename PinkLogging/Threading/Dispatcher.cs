using System;
using System.Collections.Concurrent;

namespace PinkLogging.Threading
{
    public sealed class Dispatcher : SynchronizationContext, IDisposable
    {
        private readonly Thread _thread = Thread.CurrentThread;
        private readonly BlockingCollection<DispatcherOperation> _queue = new BlockingCollection<DispatcherOperation>();
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private bool _shutdown;

        public Dispatcher() : this("Dispatcher")
        {
        }

        public Dispatcher(string name)
        {
            _thread = new Thread(() =>
            {
                SetSynchronizationContext(this);
                Run();
            });
            _thread.Name = name;
            _thread.Start();
        }

        ~Dispatcher()
        {
            Dispose();
        }

        public override SynchronizationContext CreateCopy()
        {
            return new Dispatcher();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Add(new DispatcherPostOperation(d, state));
            Continue();
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (Thread.CurrentThread == _thread)
            {
                d(state);
            }
            else
            {
                using (var operation = new DispatcherSendOperation(d, state))
                {
                    _queue.Add(operation);
                    Continue();
                    operation.Complete();
                }
            }
        }

        private void Continue()
        {
            lock (_waitHandle)
            {
                _waitHandle.Set();
            }
        }

        public void Run()
        {
            VerifyAccess();

            while (true)
            {
                foreach (var operation in _queue.GetConsumingEnumerable())
                    operation.Execute();
                
                if (_shutdown)
                    break;

                lock (_waitHandle)
                {
                    _waitHandle.Reset();
                    _waitHandle.WaitOne();
                }
            }
        }

        public void Shutdown()
        {
            _shutdown = true;
            _queue.CompleteAdding();
            Continue();
        }

        public void VerifyAccess()
        {
            if (RequireInvoke())
                throw new InvalidOperationException("Cross thread access");
        }

        public bool RequireInvoke()
        {
            return Thread.CurrentThread != _thread;
        }

        public void Dispose()
        {
            Shutdown();

            GC.SuppressFinalize(this);
        }
    }
}
