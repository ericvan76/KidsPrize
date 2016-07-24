using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KidsPrize.Http.Bus
{
    public class SimpleBus : IBus
    {
        private readonly IServiceProvider _serviceProvider;
        public SimpleBus(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task Send<T>(T command) where T : Command
        {
            var handlers = this._serviceProvider.GetServices<IHandleMessages<T>>();
            if (handlers.Count() == 0)
            {
                throw new InvalidOperationException($"No handler registered for {typeof(T).FullName}");
            }
            var handlerExceptions = new List<Exception>();
            foreach (var handler in handlers)
            {
                try
                {
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