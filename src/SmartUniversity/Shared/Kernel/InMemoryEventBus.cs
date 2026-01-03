using System.Collections.Concurrent;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Shared.Kernel.Service
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

        public Task PublishAsync<TEvent>(TEvent @event)
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                var tasks = new List<Task>();
                foreach (var handler in handlers)
                {
                    tasks.Add(handler(@event));
                }
                return Task.WhenAll(tasks);
            }
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent>(Func<TEvent, Task> handler)
        {
            var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => new List<Func<object, Task>>());
            handlers.Add(e => handler((TEvent)e));
        }
    }
}
