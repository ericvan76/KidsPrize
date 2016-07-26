using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;

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
            var child = await this._context.GetChildOrThrow(command.UserId(), command.ChildId);
            child.Update(command.Name, command.Gender, null);
            await _context.SaveChangesAsync();
        }
    }
}