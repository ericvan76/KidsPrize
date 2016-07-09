using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class UpdateChild : Command
    {
        [Required]
        public Guid ChildUid { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^(Male|Female)$")]
        public string Gender { get; set; }
    }

    public class UpdateChildHandler : IHandleMessages<UpdateChild>
    {
        private KidsPrizeDbContext _context;
        public UserInfo User { get; set; }

        public UpdateChildHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }

        public async Task Handle(UpdateChild command)
        {
            var userUid = User.Uid;
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            var child = user.Children.FirstOrDefault(i => i.Uid == command.ChildUid);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.ChildUid} not found.");
            }
            child.Update(command.Name, command.Gender, null);
            await _context.SaveChangesAsync();
        }
    }
}