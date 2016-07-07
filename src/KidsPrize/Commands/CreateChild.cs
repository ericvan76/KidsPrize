using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class CreateChild : Command
    {
        [Required]
        public Guid ChildUid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^(Male|Female)$")]
        public string Gender { get; set; }
    }

    public class CreateChildHandler : IHandleMessages<CreateChild>
    {
        private readonly KidsPrizeDbContext _context;
        public UserInfo User { get; set; }
        public CreateChildHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }

        public async Task Handle(CreateChild command)
        {
            var userUid = User.Uid;
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            user.AddChild(new Child(0, command.ChildUid, command.Name, command.Gender, 0));
            await _context.SaveChangesAsync();
        }
    }
}