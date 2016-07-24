using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;

namespace KidsPrize.Commands
{
    public class CreateChild : Command
    {
        [Required]
        public Guid ChildId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^(Male|Female)$")]
        public string Gender { get; set; }
    }

    public class CreateChildHandler : IHandleMessages<CreateChild>
    {
        private readonly KidsPrizeContext _context;

        public CreateChildHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task Handle(CreateChild command)
        {
            this._context.Add(new Child(command.ChildId, command.UserId(), command.Name, command.Gender, 0));
            await _context.SaveChangesAsync();
        }
    }
}