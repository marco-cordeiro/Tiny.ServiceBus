using System.Threading.Tasks;

namespace Tiny.ServiceBus
{
    public interface IHandleMessage<in TMessage>
    {
        Task Handle(TMessage message);
    }
}