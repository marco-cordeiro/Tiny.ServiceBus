using System.Threading.Tasks;

namespace Tiny.ServiceBus
{
    public interface IHandleMessage<TMessage>
    {
        Task Handle(TMessage message);
    }
}