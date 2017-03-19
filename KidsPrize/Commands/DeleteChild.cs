using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Extensions;
using MediatR;

namespace KidsPrize.Commands
{
    public class DeleteChild : Command, IRequest
    {
        [Required]
        public Guid ChildId { get; set; }
    }

    public class DeleteChildHandler : IAsyncRequestHandler<DeleteChild>
    {
        private readonly KidsPrizeContext _context;

        public DeleteChildHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task Handle(DeleteChild message)
        {
            var child = await this._context.GetChildOrThrow(message.UserId(), message.ChildId);

            this._context.Children.Remove(child);

            await _context.SaveChangesAsync();
        }

    }
}