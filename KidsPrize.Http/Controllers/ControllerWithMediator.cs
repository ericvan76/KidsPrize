using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [Produces("application/json")]
    public abstract class ControllerWithMediator : VersionedController
    {
        private readonly IMediator _mediator;

        public ControllerWithMediator(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task Send<TCommand>(TCommand command) where TCommand : Command, IRequest
        {
            command.SetHeader("Authorisation", this.User);
            await this._mediator.Send(command);
        }

        public async Task<TResponse> Send<TCommand, TResponse>(TCommand command) where TCommand : Command, IRequest<TResponse>
        {
            command.SetHeader("Authorisation", this.User);
            return await this._mediator.Send<TResponse>(command);
        }

    }
}