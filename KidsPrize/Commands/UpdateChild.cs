using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Extensions;
using KidsPrize.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Models;
using R = KidsPrize.Resources;

namespace KidsPrize.Commands
{
    public class UpdateChild : Command, IRequest<R.ScoreResult>
    {
        [Required]
        public Guid ChildId { get; set; }

        public string Name { get; set; }

        [RegularExpression(@"^(M|F)$")]
        public string Gender { get; set; }

        public string[] Tasks { get; set; }
    }

    public class UpdateChildHandler : IAsyncRequestHandler<UpdateChild, R.ScoreResult>
    {
        private readonly KidsPrizeContext _context;
        private readonly IScoreService _scoreService;

        public UpdateChildHandler(KidsPrizeContext context, IScoreService scoreService)
        {
            this._context = context;
            this._scoreService = scoreService;
        }

        public async Task<R.ScoreResult> Handle(UpdateChild message)
        {

            var child = await this._context.GetChildOrThrow(message.UserId(), message.ChildId);
            child.Update(message.Name, message.Gender, null);

            var preference = await this._context.GetPreference(message.UserId());

            if (message.Tasks != null && message.Tasks.Length > 0)
            {
                await UpdateTasks(child, message, preference.Today().StartOfWeek());
            }

            await _context.SaveChangesAsync();

            return await this._scoreService.GetScores(message.UserId(), message.ChildId, preference.Today().StartOfNextWeek(), 1);
        }

        private async Task UpdateTasks(E.Child child, UpdateChild message, DateTime effectiveDate)
        {
            var existingTaskgroup = await this._context.GetTaskGroup(child.Id, effectiveDate);

            if (!existingTaskgroup.Tasks.OrderBy(t => t.Order).Select(t => t.Name).SequenceEqual(message.Tasks, StringComparer.OrdinalIgnoreCase))
            {
                if (existingTaskgroup.EffectiveDate == effectiveDate)
                {
                    existingTaskgroup.Update(message.Tasks);
                }
                else
                {
                    var taskGroup = new E.TaskGroup(child, effectiveDate, message.Tasks);
                    this._context.TaskGroups.Add(taskGroup);
                }

                // delete scores of removed Tasks
                var endDate = effectiveDate.AddDays(7);
                var removed = await this._context.Scores.Where(s =>
                    s.Child.Id == message.ChildId &&
                    s.Date >= effectiveDate &&
                    s.Date < endDate &&
                    !message.Tasks.Contains(s.Task)
                ).ToListAsync();
                var delta = removed.Sum(s => s.Value);
                this._context.RemoveRange(removed);
                child.Update(null, null, child.TotalScore - delta);
            }
        }
    }
}