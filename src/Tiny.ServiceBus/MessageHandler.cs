using System.Threading.Tasks;

namespace Tiny.ServiceBus
{
    internal class MessageHandler<TMessage> : IMessageHandler
    {
        private readonly IHandleMessage<TMessage> _handler;
        private readonly TMessage _message;

        public MessageHandler(IHandleMessage<TMessage> handler, TMessage message)
        {
            _handler = handler;
            _message = message;
        }

        public Task Invoke()
        {
            return _handler.Handle(_message);
        }
    }
}