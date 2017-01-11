using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Extensions;
using MediatR;

namespace KidsPrize.Commands
{
    public class DeleteChild : Command, IAsyncRequest
    {
        [Required]
        public Guid ChildId { get; set; }
    }

    public class DeleteChildHandler : AsyncRequestHandler<DeleteChild>
    {
        private readonly KidsPrizeContext _context;

        public DeleteChildHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        protected override async Task HandleCore(DeleteChild message)
        {
            var child = await this._context.GetChildOrThrow(message.UserId(), message.ChildId);

            this._context.Children.Remove(child);

            await _context.SaveChangesAsync();
        }
    }
}