using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class DeleteChild : Command
    {
        [Required]
        public Guid ChildUid { get; set; }
    }

    public class DeleteChildHandler : IHandleMessages<DeleteChild>, IHasUser
    {
        private readonly KidsPrizeDbContext _context;
        public ClaimsPrincipal User { get; set; }

        public DeleteChildHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }
        public async Task Handle(DeleteChild command)
        {
            var userUid = User.UserUid();
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            user.RemoveChild(command.ChildUid);
            await _context.SaveChangesAsync();
        }
    }
}