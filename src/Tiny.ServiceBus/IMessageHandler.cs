using System.Threading.Tasks;

namespace Tiny.ServiceBus
{
    public interface IMessageHandler
    {
        Task Invoke();
    }
}