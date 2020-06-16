using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tiny.ServiceBus.Tests
{
    public class MessageHandlerRegistryTest
    {
        [Fact]
        public void Getting_Handlers_Calls_ServiceProvider_To_Create_Handler_Instance()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var registry = new MessageHandlerRegistry(serviceProviderMock.Object);
            registry.ScanAssembly(GetType().Assembly);

            var result = registry.GetHandlerFor(new TinyMessage());

            serviceProviderMock.Verify(x => x.GetService(typeof(TinyMessageHandler)), Times.Once);
        }
    }

    public class TinyMessageHandler : IHandleMessage<TinyMessage>
    {
        public Task Handle(TinyMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
