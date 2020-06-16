using System;
using System.Collections.Generic;

namespace Tiny.ServiceBus.Tests
{
    public class TestableMessageHandlerRegistry : MessageHandlerRegistry
    {
        public TestableMessageHandlerRegistry(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        public IEnumerable<IHandleMessage<TMessage>> GetHandlers<TMessage>()
        {
            return GetHandlerInstances<TMessage>();
        }
    }
}
