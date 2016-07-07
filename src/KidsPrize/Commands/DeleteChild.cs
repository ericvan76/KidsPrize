using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class DeleteChild : Command
    {
        [Required]
        public Guid ChildUid { get; set; }
    }

    public class DeleteChildHandler : IHandleMessages<DeleteChild>
    {
        private readonly KidsPrizeDbContext _context;
        public UserInfo User { get; set; }

        public DeleteChildHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }
        public async Task Handle(DeleteChild command)
        {
            var userUid = User.Uid;
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            user.RemoveChild(command.ChildUid);
            await _context.SaveChangesAsync();
        }
    }
}