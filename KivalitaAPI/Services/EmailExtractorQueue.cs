using System;
using System.Collections.Concurrent;
using System.Threading;

namespace KivalitaAPI.Services
{
    public class EmailExtractorQueue
    {

        private BlockingCollection<object> _jobs = new BlockingCollection<object>();

        public EmailExtractorQueue()
        {
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = false;
            thread.Start();
        }

        public void Enqueue(Action func)
        {
            _jobs.Add(func);
        }

        private void OnStart()
        {
            foreach (Action func in _jobs.GetConsumingEnumerable(CancellationToken.None))
            {
                func();
             }
        }
    }
}
