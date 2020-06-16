using System.Threading.Tasks;

namespace Tiny.ServiceBus
{
    public interface IServiceBus
    {
        Task Publish<TMessage>(TMessage message);
    }
}