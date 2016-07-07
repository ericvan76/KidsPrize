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
        private readonly UserInfo _user;

        public TestBus(UserInfo user, params object[] handlers)
        {
            _user = user;
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
                    handler.User = this._user;
                    await handler.Handle(command).ConfigureAwait(false);
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