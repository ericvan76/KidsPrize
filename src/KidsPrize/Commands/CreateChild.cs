using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Extensions;
using KidsPrize.Services;
using MediatR;
using E = KidsPrize.Models;
using R = KidsPrize.Resources;

namespace KidsPrize.Commands
{
    public class CreateChild : Command, IAsyncRequest<R.ScoreResult>
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(M|F)$")]
        public string Gender { get; set; }

        [Required]
        [MinLength(1)]
        public string[] Tasks { get; set; }

    }

    public class CreateChildHandler : IAsyncRequestHandler<CreateChild, R.ScoreResult>
    {
        private readonly KidsPrizeContext _context;
        private readonly IScoreService _scoreService;

        public CreateChildHandler(KidsPrizeContext context, IScoreService scoreService)
        {
            this._context = context;
            this._scoreService = scoreService;
        }

        public async Task<R.ScoreResult> Handle(CreateChild message)
        {
            var child = new E.Child(message.ChildId, message.UserId(), message.Name, message.Gender);
            var preference = await this._context.GetPreference(message.UserId());

            var taskGroup = new E.TaskGroup(
                child,
                preference.Today().StartOfWeek(),
                message.Tasks);

            this._context.Children.Add(child);
            this._context.TaskGroups.Add(taskGroup);

            await _context.SaveChangesAsync();

            return await this._scoreService.GetScores(message.UserId(), message.ChildId, preference.Today().StartOfNextWeek(), 1);
        }
    }
}