using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tiny.ServiceBus.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ServiceCollection();
            builder.AddSingleton<IServiceBus, ServiceBus>();
            builder.AddSingleton<IMessageHandlerRegistry, MessageHandlerRegistry>();
            builder.AddTransient<MyMessageHandler>();

            var serviceProvider = builder.BuildServiceProvider();

            var registry = serviceProvider.GetService<IMessageHandlerRegistry>();
            var serviceBus = serviceProvider.GetService<IServiceBus>();

            registry.ScanAssembly(Assembly.GetExecutingAssembly());

            string input;
            do
            {
                Console.Write($"[{Thread.CurrentThread.ManagedThreadId}] type a message :");
                input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    serviceBus.Publish(new MyMessage(input));
                    //for (var a = 0; a < 10; a++)
                    //{
                    //    serviceBus.Publish(new MyMessage($"({a}) - {input}"));
                    //}
                }

            } while (!string.IsNullOrWhiteSpace(input));
        }
    }

    public class MyMessageHandler : IHandleMessage<MyMessage>
    {
        public Task Handle(MyMessage message)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] {message.Message}");

            //block the thread
            Thread.Sleep(1000);

            return Task.CompletedTask;
        }
    }

    public class MyMessage
    {
        public string Message { get; }

        public MyMessage(string input)
        {
            Message = input;
        }
    }
}
