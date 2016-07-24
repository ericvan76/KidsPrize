using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class DeleteChild : Command
    {
        [Required]
        public Guid ChildId { get; set; }
    }

    public class DeleteChildHandler : IHandleMessages<DeleteChild>
    {
        private readonly KidsPrizeContext _context;

        public DeleteChildHandler(KidsPrizeContext context)
        {
            this._context = context;
        }
        public async Task Handle(DeleteChild command)
        {
            var child = await _context.Children.FirstOrDefaultAsync(i => i.UserId == command.UserId() && i.Id == command.ChildId);
            if (child != null)
            {
                this._context.Children.Remove(child);
                await _context.SaveChangesAsync();
            }
        }
    }
}