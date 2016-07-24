using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KidsPrize.Bus;

namespace KidsPrize.Tests.Common
{
    public class TestBus : IBus
    {
        private readonly IList<object> _handlers = new List<object>();

        public TestBus(params object[] handlers)
        {
            _handlers = handlers;
        }
        public async Task Send<T>(T command) where T : Command
        {
            var handlers = this._handlers.Where(i => i.GetType().GetInterfaces().Contains(typeof(IHandleMessages<T>)));
            if (handlers.Count() == 0)
            {
                throw new InvalidOperationException($"No handler registered for {typeof(T).FullName}");
            }
            var handlerExceptions = new List<Exception>();
            foreach (IHandleMessages<T> handler in handlers)
            {
                try
                {
                    await handler.Handle(command);
                }
                catch (Exception ex)
                {
                    handlerExceptions.Add(ex);
                }
            }
            if (handlerExceptions.Any())
            {
                throw new AggregateException(handlerExceptions);
            }
        }
    }
}