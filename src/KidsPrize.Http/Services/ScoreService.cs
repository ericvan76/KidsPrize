using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Http.Commands;
using KidsPrize.Http.Models;
using KidsPrize.Repository.Npgsql;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Repository.Npgsql.Entities;

namespace KidsPrize.Http.Services
{
    public interface IScoreService
    {
        Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks);
        Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId);
        Task SetScore(string userId, SetScoreCommand command);
    }

    public class ScoreService : IScoreService
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;

        public ScoreService(KidsPrizeContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks)
        {
            var dateFrom = rewindFrom.AddDays(-7 * numOfWeeks);

            // todo: improve this query
            var child = await this._context.GetChildOrThrow(userId, childId);
            var scores = await this._context.Scores.Where(s => s.Child.Id == childId && s.Date >= dateFrom && s.Date < rewindFrom).ToListAsync();
            var taskGroups = (await this._context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == childId && tg.EffectiveDate > dateFrom && tg.EffectiveDate < rewindFrom).ToListAsync())
                .Union(await this._context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == childId && tg.EffectiveDate <= dateFrom).OrderByDescending(tg => tg.EffectiveDate).Take(1).ToListAsync())
                .ToList();

            var weeklyScoresList = new List<WeeklyScores>();
            for (int i = 1; i <= numOfWeeks; i++)
            {
                var weekStart = rewindFrom.AddDays(-7 * i);
                var weekEnd = weekStart.AddDays(7);
                var taskGroup = taskGroups.OrderByDescending(tg => tg.EffectiveDate).FirstOrDefault(tg => tg.EffectiveDate <= weekStart);
                if (taskGroup != null)
                {
                    weeklyScoresList.Add(new WeeklyScores()
                    {
                        Week = weekStart,
                        Tasks = taskGroup.Tasks.OrderBy(t => t.Order).Select(t => t.Name).ToList(),
                        Scores = scores.Where(s => s.Date >= weekStart && s.Date < weekEnd).Select(s => _mapper.Map<Score>(s)).ToList()
                    });
                }
            }

            return new ScoreResult
            {
                Child = _mapper.Map<Child>(child),
                WeeklyScores = weeklyScoresList
            };
        }

        public async Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId)
        {
            var preference = await this._context.GetPreference(userId);
            return await this.GetScores(userId, childId, preference.Today().StartOfNextWeek(), 1);
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