using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tiny.ServiceBus.Tests
{
    public class ServiceBusTest
    {
        [Fact]
        public void Publish_Gets_Handlers_From_Registry()
        {
            var registryMock = new Mock<IMessageHandlerRegistry>();
            var sut = new ServiceBus(registryMock.Object);

            sut.Publish(new TinyMessage());

            registryMock.Verify(x => x.GetHandlerFor(It.IsAny<TinyMessage>()), Times.Once);
        }

        [Fact]
        public async Task EventProcess_Call_Handler_Invoke()
        {
            var handlerMock = new Mock<IMessageHandler>();
            var registryMock = new Mock<IMessageHandlerRegistry>();
            var semaphore = new SemaphoreSlim(0, 1);

            registryMock.Setup(x => x.GetHandlerFor(It.IsAny<TinyMessage>()))
                .Returns(() => new[] { handlerMock.Object });

            handlerMock.Setup(x => x.Invoke());

            var sut = new ServiceBus(registryMock.Object);

            await sut.Publish(new TinyMessage());

            sut.Completion().Wait(500);

            handlerMock.Verify(x => x.Invoke(), Times.Once);
        }
    }
}
