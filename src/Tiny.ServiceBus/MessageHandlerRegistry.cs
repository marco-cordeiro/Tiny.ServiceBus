using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tiny.ServiceBus
{
    public class MessageHandlerRegistry : IMessageHandlerRegistry
    {
        private readonly IDictionary<Type, List<Type>> _subscriptions;
        private readonly IServiceProvider _serviceProvider;

        public MessageHandlerRegistry(IServiceProvider serviceProvider)
        {
            _subscriptions = new Dictionary<Type, List<Type>>();
            _serviceProvider = serviceProvider;
        }

        public void ScanAssembly(Assembly assembly)
        {
            var types = RegisterSubscriptions(assembly);
            foreach(var handlerType in types)
            {
                var messageTypes = handlerType.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandleMessage<>))
                    .Select(x=>x.GetGenericArguments().First()).ToArray();
                
                foreach(var messageType in messageTypes)
                {
                    if (_subscriptions.ContainsKey(messageType))
                    {
                        _subscriptions[messageType].Add(handlerType);
                        continue;
                    }
                    _subscriptions[messageType] = new List<Type> { handlerType };
                }
            }
        }

        public IEnumerable<IMessageHandler> GetHandlerFor<TMessage>(TMessage message)
        {
            var handlers = GetHandlerInstances<TMessage>();
            foreach(var handler in handlers)
            {
                yield return new MessageHandler<TMessage>(handler, message);
            }            
        }

        protected IEnumerable<IHandleMessage<TMessage>> GetHandlerInstances<TMessage>()
        {
            if (!_subscriptions.TryGetValue(typeof(TMessage), out var handlerTypes))
                yield break;

            foreach (var handlerType in handlerTypes)
            {
                yield return (IHandleMessage<TMessage>)_serviceProvider.GetService(handlerType);
            }
        }

        private Type[] RegisterSubscriptions(Assembly assembly)
        {
            var types = assembly.DefinedTypes
                .Where(type => type.AsType().GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandleMessage<>))).ToArray();

            foreach(var messageType in types)
            {
                var handlers = messageType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandleMessage<>)).ToArray();
                    //.Select(x => x.)
            }

            return types;
        }
    }
}