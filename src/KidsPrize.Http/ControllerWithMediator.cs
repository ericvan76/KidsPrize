using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace KidsPrize.Http
{
    public class ControllerWithMediator : Controller
    {
        private readonly IMediator _mediator;

        public ControllerWithMediator(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task Send<TCommand>(TCommand command) where TCommand : Command, IAsyncRequest
        {
            command.SetHeader("Authorisation", this.User);
            await this._mediator.SendAsync(command);
        }

        public async Task<TResponse> Send<TCommand, TResponse>(TCommand command) where TCommand : Command, IAsyncRequest<TResponse>
        {
            command.SetHeader("Authorisation", this.User);
            return await this._mediator.SendAsync<TResponse>(command);
        }

    }
}