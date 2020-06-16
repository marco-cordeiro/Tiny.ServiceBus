using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tiny.ServiceBus
{
    public class ServiceBus : IServiceBus
    {
        private readonly ActionBlock<ActionCommand> _actionBlock;
        protected readonly ConcurrentDictionary<Type, IList<Delegate>> _subscriptions;
        private readonly IMessageHandlerRegistry _registry;

        public ServiceBus(IMessageHandlerRegistry registry)
        {
            var options = new ExecutionDataflowBlockOptions();
            _actionBlock = new ActionBlock<ActionCommand>(ProcessMessage, options);
            _registry = registry;
        }

        public Task Publish<TMessage>(TMessage message)
        {
            var handlers = _registry.GetHandlerFor(message);
            return _actionBlock.SendAsync(new ActionCommand(handlers));
        }

        public Task Completion()
        {
            return _actionBlock.Completion;
        }

        private async Task ProcessMessage(ActionCommand command)
        {
            foreach(var handler in command.Handlers)
            {
                try
                {
                    await handler.Invoke();
                }
                catch
                {
                    // exception needs to be caught here otherwise will bubble up and potentially crash the process
                    // logging exception here might be a good idea
                }
            }
        }

        private class ActionCommand
        {
            public ActionCommand(IEnumerable<IMessageHandler> messageHandlers)
            {
                Handlers = messageHandlers.ToArray();
            }
            public IMessageHandler[] Handlers { get; }
        }
    }
}