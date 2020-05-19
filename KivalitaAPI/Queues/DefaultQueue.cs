using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace KivalitaAPI.Queues
{
    public class DefaultQueue
    {
        private BlockingCollection<Func<Task>> _jobs = new BlockingCollection<Func<Task>>();

        public DefaultQueue()
        {
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = false;
            thread.Start();

        }

        public void Enqueue(Func<Task> func)
        {
            _jobs.Add(func);
        }

        private void OnStart()
        {
            foreach (Func<Task> func in _jobs.GetConsumingEnumerable(CancellationToken.None))
            {
                var f = func();
                f.Wait();

                Console.WriteLine($"[queue] - Number of jobs in the queue: {_jobs.Count}");
            }

        }
    }
}
