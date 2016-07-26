using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Extensions;
using KidsPrize.Models;
using KidsPrize.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsPrize.Services
{
    public interface IScoreService
    {
        Task<WeekScores> GetWeekScores(Guid userId, Guid childId, DateTime date);
    }

    public class ScoreService : IScoreService
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        private readonly DefaultTasks _defaultTasks;

        public ScoreService(KidsPrizeContext context, IMapper mapper, IOptions<DefaultTasks> defaultTasks)
        {
            this._context = context;
            this._mapper = mapper;
            this._defaultTasks = defaultTasks.Value;
        }

        public async Task<WeekScores> GetWeekScores(Guid userId, Guid childId, DateTime date)
        {
            var child = await this._context.GetChildOrThrow(userId, childId);
            var days = await this._context.ResolveRecentDays(child, date);

            var defaultTasks = days?.FirstOrDefault()?.TaskList ?? _defaultTasks;

            var result = new List<Day>();
            var startOfWeek = date.StartOfWeek();
            for (int idx = 0; idx < 7; idx++)
            {
                var d = startOfWeek.AddDays(idx);
                result.Add(days?.FirstOrDefault(i => i.Date == d) ?? new Day(0, child, d, defaultTasks.ToArray()));
            }
            return new WeekScores()
            {
                ChildId = child.Id,
                ChildTotal = child.TotalScore,
                DayScores = result.Select(d => _mapper.Map<Day, DayScore>(d))
            };
        }
    }

    public class DefaultTasks : List<string>
    {
    }
}