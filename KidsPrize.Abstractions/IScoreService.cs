using System;
using System.Threading.Tasks;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;

namespace KidsPrize.Abstractions
{
    public interface IScoreService
    {
        Task<ScoreResult> GetScores(string userId, Guid childId, DateTime rewindFrom, int numOfWeeks);
        Task<ScoreResult> GetScoresOfCurrentWeek(string userId, Guid childId);
        Task SetScore(string userId, SetScore command);
    }
}