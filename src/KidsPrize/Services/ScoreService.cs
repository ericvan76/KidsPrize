using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Data;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Data.Entities;

namespace KidsPrize.Services
{
    public interface IScoreService
    {
        Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks);
        Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId, DateTime today);
        Task SetScore(string userId, SetScoreCommand command);
    }

    public class ScoreService : IScoreService
    {
        private readonly KidsPrizeContext _context;

        public ScoreService(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks)
        {
            var dateFrom = rewindFrom.AddDays(-7 * numOfWeeks);

            // todo: improve this query
            var child = await this._context.GetChildOrThrow(userId, childId);
            var scores = await this._context.Scores.AsNoTracking()
                .Where(s => s.Child.Id == childId && s.Date >= dateFrom && s.Date < rewindFrom)
                .ToListAsync();
            var taskGroups = await this._context.TaskGroups.AsNoTracking().Include(tg => tg.Tasks)
                .Where(tg => tg.Child.Id == childId && tg.EffectiveDate < rewindFrom)
                .OrderByDescending(tg => tg.EffectiveDate)
                .Take(numOfWeeks + 1).ToListAsync();

            var weeklyScoresList = new List<WeeklyScores>();
            for (int i = 1; i <= numOfWeeks; i++)
            {
                var weekStart = rewindFrom.AddDays(-7 * i);
                var weekEnd = weekStart.AddDays(7);
                var taskGroup = taskGroups.FirstOrDefault(tg => tg.EffectiveDate <= weekStart);
                if (taskGroup != null)
                {
                    weeklyScoresList.Add(new WeeklyScores()
                    {
                        Week = weekStart,
                        Tasks = taskGroup.Tasks.OrderBy(t => t.Order).Select(t => t.Name).ToList(),
                        Scores = scores.Where(s => s.Date >= weekStart && s.Date < weekEnd)
                        .Select(s => new Score
                        {
                            Date = s.Date,
                            Task = s.Task,
                            Value = s.Value
                        }).ToList()
                    });
                }
            }

            return new ScoreResult
            {
                Child = new Child
                {
                    Id = child.Id,
                    Name = child.Name,
                    Gender = child.Gender,
                    TotalScore = child.TotalScore
                },
                WeeklyScores = weeklyScoresList
            };
        }

        public async Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId, DateTime today)
        {
            return await this.GetScores(userId, childId, today.StartOfNextWeek(), 1);
        }

        public async Task SetScore(string userId, SetScoreCommand command)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);

            // Validate Task
            var taskGroup = await this._context.GetTaskGroup(child.Id, command.Date);
            if (taskGroup == null || !taskGroup.Tasks.Any(t => t.Name.Equals(command.Task, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var score = this._context.Scores.Include(s => s.Child)
                .FirstOrDefault(s => s.Child.Id == command.ChildId && s.Date == command.Date && s.Task == command.Task);

            if (score == null)
            {
                score = new E.Score(child, command.Date, command.Task, 0);
                this._context.Scores.Add(score);
            }

            score.Update(command.Value);

            await this._context.SaveChangesAsync();
        }
    }
}