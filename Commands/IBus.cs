using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public class Command
{
    public ClaimsPrincipal User { get; set; }
}

public interface IBus
{
    Task Send<T>(T command) where T : Command;
}

public interface IHandleMessages<TCommand> where TCommand : Command
{
    Task Handle(TCommand command);
}

public interface IHasUser
{
    ClaimsPrincipal User { get; set; }
}

public class Bus : IBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Bus(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
    {
        this._serviceProvider = serviceProvider;
        this._httpContextAccessor = httpContextAccessor;
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
                var hasUser = handler as IHasUser;
                if (hasUser != null)
                {
                    hasUser.User = _httpContextAccessor.HttpContext.User;
                }
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