using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Extensions;
using Microsoft.EntityFrameworkCore;
using R = KidsPrize.Resources;

namespace KidsPrize.Services
{
    public interface IScoreService
    {
        Task<R.ScoreResult> GetScores(Guid userId, Guid childId, DateTime rewindFrom, int numOfWeeks);
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

        public async Task<R.ScoreResult> GetScores(Guid userId, Guid childId, DateTime rewindFrom, int numOfWeeks)
        {
            var dateFrom = rewindFrom.AddDays(-7 * numOfWeeks);

            // todo: improve this
            var child = await this._context.GetChildOrThrow(userId, childId);
            var scores = await this._context.Scores.Where(s => s.Child.Id == childId && s.Date >= dateFrom && s.Date < rewindFrom).ToListAsync();
            var taskGroups = (await this._context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == childId && tg.EffectiveDate > dateFrom && tg.EffectiveDate < rewindFrom).ToListAsync())
                .Union(await this._context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == childId && tg.EffectiveDate <= dateFrom).OrderByDescending(tg => tg.EffectiveDate).Take(1).ToListAsync());

            return new R.ScoreResult
            {
                Child = _mapper.Map<R.Child>(child),
                Scores = scores.Select(s =>_mapper.Map<R.Score>(s)).ToList(),
                TaskGroups = taskGroups.Select(t=>_mapper.Map<R.TaskGroup>(t)).ToList()
            };
        }
    }
}