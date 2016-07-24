using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class UpdateChild : Command
    {
        [Required]
        public Guid ChildId { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^(Male|Female)$")]
        public string Gender { get; set; }
    }

    public class UpdateChildHandler : IHandleMessages<UpdateChild>
    {
        private KidsPrizeContext _context;

        public UpdateChildHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task Handle(UpdateChild command)
        {
            var child = await _context.Children.FirstOrDefaultAsync(i => i.UserId == command.UserId() && i.Id == command.ChildId);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.ChildId} not found.");
            }
            child.Update(command.Name, command.Gender, null);
            await _context.SaveChangesAsync();
        }
    }
}