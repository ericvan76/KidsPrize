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
    public interface IChildService
    {
        Task<Child> GetChild(string userId, Guid childId);
        Task<IEnumerable<Child>> GetChildren(string userId);
        Task<Child> CreateChild(string userId, CreateChildCommand command, DateTime today);
        Task<Child> UpdateChild(string userId, UpdateChildCommand command, DateTime today);
        Task DeleteChild(string userId, Guid childId);
        Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks);
        Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId, DateTime today);
        Task SetScore(string userId, SetScoreCommand command);
        Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset);
        Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command);
    }

    public class ChildService : IChildService
    {
        private readonly KidsPrizeContext _context;
        public ChildService(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task<Child> CreateChild(string userId, CreateChildCommand command, DateTime today)
        {
            var child = new E.Child(command.ChildId, userId, command.Name, command.Gender);

            var taskGroup = new E.TaskGroup(
                child,
                today.StartOfWeek(),
                command.Tasks);

            this._context.Children.Add(child);
            this._context.TaskGroups.Add(taskGroup);

            await _context.SaveChangesAsync();

            return MapChild(child);
        }

        public async Task DeleteChild(string userId, Guid childId)
        {
            var child = await this._context.GetChildOrThrow(userId, childId);
            this._context.Children.Remove(child);

            await _context.SaveChangesAsync();
        }

        public async Task<Child> GetChild(string userId, Guid childId)
        {
            var child = await _context.GetChild(userId, childId);
            if (child != null)
            {
                return MapChild(child);
            }
            return null;
        }

        public async Task<IEnumerable<Child>> GetChildren(string userId)
        {
            var result = new List<Child>();
            var children = await _context.Children.AsNoTracking().Where(i => i.UserId == userId).ToListAsync();
            children.ForEach(i => result.Add(MapChild(i)));
            return result;
        }

        public async Task<Child> UpdateChild(string userId, UpdateChildCommand command, DateTime today)
        {
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);
            child.Update(command.Name, command.Gender, null);

            if (command.Tasks != null && command.Tasks.Length > 0)
            {
                await UpdateTasks(child, command.Tasks, today.StartOfWeek());
            }

            await _context.SaveChangesAsync();
            return MapChild(child);
        }

        private async Task UpdateTasks(E.Child child, string[] tasks, DateTime effectiveDate)
        {
            var existingTaskgroup = await this._context.GetTaskGroup(child.Id, effectiveDate);

            if (!existingTaskgroup.Tasks.OrderBy(t => t.Order).Select(t => t.Name).SequenceEqual(tasks, StringComparer.OrdinalIgnoreCase))
            {
                if (existingTaskgroup.EffectiveDate == effectiveDate)
                {
                    existingTaskgroup.Update(tasks);
                }
                else
                {
                    var taskGroup = new E.TaskGroup(child, effectiveDate, tasks);
                    this._context.TaskGroups.Add(taskGroup);
                }

                // delete scores of removed Tasks
                var endDate = effectiveDate.AddDays(7);
                var removed = await this._context.Scores.Where(s =>
                    s.Child.Id == child.Id &&
                    s.Date >= effectiveDate &&
                    s.Date < endDate &&
                    !tasks.Contains(s.Task)
                ).ToListAsync();
                var delta = removed.Sum(s => s.Value);
                this._context.RemoveRange(removed);
                child.Update(null, null, child.TotalScore - delta);
            }
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

        public async Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);
            var redeem = new E.Redeem(child, DateTimeOffset.Now, command.Description, command.Value);

            child.Update(null, null, child.TotalScore - command.Value);
            this._context.Redeems.Add(redeem);
            await _context.SaveChangesAsync();

            return new Redeem
            {
                Timestamp = redeem.Timestamp,
                Description = redeem.Description,
                Value = redeem.Value
            };
        }

        public async Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset)
        {
            var query = await this._context.Redeems.Where(i => i.Child.Id == childId).OrderByDescending(i => i.Timestamp)
                .Skip(offset).Take(limit).ToListAsync();
            return query.Select(i => new Redeem
            {
                Timestamp = i.Timestamp,
                Description = i.Description,
                Value = i.Value
            });
        }

        private Child MapChild(E.Child child)
        {
            return new Child
            {
                Id = child.Id,
                Name = child.Name,
                Gender = child.Gender,
                TotalScore = child.TotalScore
            };
        }
    }
}