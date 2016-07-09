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
        Task<WeekScores> GetWeekScores(Guid userUid, Guid childUid, DateTime date);
    }

    public class ScoreService : IScoreService
    {
        private readonly KidsPrizeDbContext _context;
        private readonly IMapper _mapper;
        private readonly DefaultTasks _defaultTasks;

        public ScoreService(KidsPrizeDbContext context, IMapper mapper, IOptions<DefaultTasks> defaultTasks)
        {
            this._context = context;
            this._mapper = mapper;
            this._defaultTasks = defaultTasks.Value;
        }

        public async Task<WeekScores> GetWeekScores(Guid userUid, Guid childUid, DateTime date)
        {
            var user = await this._context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            var child = user.Children.FirstOrDefault(i => i.Uid == childUid);
            if (child == null)
            {
                throw new ArgumentException($"Child {childUid} not found.");
            }

            var days = await this._context.Days.Include(d => d.Child).Include(d => d.Scores)
                .Where(d => d.Child.Id == child.Id && d.Date <= date.EndOfWeek())
                .OrderByDescending(d => d.Date).Take(7).ToListAsync();
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
                ChildUid = childUid,
                ChildTotal = child.TotalScore,
                DayScores = result.Select(d => _mapper.Map<Day, DayScore>(d))
            };
        }
    }

    public class DefaultTasks : List<string>
    {
    }
}