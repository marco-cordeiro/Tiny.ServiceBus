using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tiny.ServiceBus
{
    public class ServiceBus : IServiceBus
    {
        private readonly ActionBlock<ActionCommand> _actionBlock;
        private readonly IMessageHandlerRegistry _registry;

        public ServiceBus(IMessageHandlerRegistry registry)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 10
            };
            _actionBlock = new ActionBlock<ActionCommand>(ProcessMessage, options);
            _registry = registry;
        }

        public Task Publish<TMessage>(TMessage message)
        {
            var handlers = _registry.GetHandlerFor(message);
            
            return Task.WhenAll((IEnumerable<Task>)handlers.Select(x => _actionBlock.SendAsync(new ActionCommand(x))));
        }

        public Task Completion()
        {
            return _actionBlock.Completion;
        }

        private async Task ProcessMessage(ActionCommand command)
        {
                try
                {
                    await command.Handler.Invoke();
                }
                catch
                {
                    // exception needs to be caught here otherwise will bubble up and potentially crash the process
                    //TODO: logging exception here might be a good idea
                    //TODO: add a retry mechanism
                }
        }

        private class ActionCommand
        {
            public ActionCommand(IMessageHandler messageHandler)
            {
                Handler = messageHandler;
            }

            public IMessageHandler Handler { get; }
        }
    }
}