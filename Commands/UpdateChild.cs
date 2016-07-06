using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class UpdateChild : Command
    {
        [Required]
        public Guid childUid { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^(Male|Female)$")]
        public string Gender { get; set; }
    }

    public class UpdateChildHandler : IHandleMessages<UpdateChild>
    {
        private KidsPrizeDbContext _context;
        public ClaimsPrincipal User { get; set; }

        public UpdateChildHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }

        public async Task Handle(UpdateChild command)
        {
            var userUid = User.UserUid();
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            var child = user.Children.FirstOrDefault(i => i.Uid == command.childUid);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.childUid} not found.");
            }
            child.Update(command.Name, command.Gender, null);
            await _context.SaveChangesAsync();
        }
    }
}