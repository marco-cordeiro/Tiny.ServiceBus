using System.Collections.Generic;
using System.Reflection;

namespace Tiny.ServiceBus
{
    public interface IMessageHandlerRegistry
    {
        void ScanAssembly(Assembly assembly);
        IEnumerable<IMessageHandler> GetHandlerFor<TMessage>(TMessage message);
    }
}