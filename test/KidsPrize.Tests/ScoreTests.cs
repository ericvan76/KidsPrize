using System;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Services;
using KidsPrize.Data;
using Xunit;

namespace KidsPrize.Tests
{
    public class ScoreTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IChildService _childService;
        private readonly IScoreService _scoreService;
        private readonly string _userId;

        public ScoreTests()
        {
            _context = TestHelper.CreateContext();
            _childService = new ChildService(_context);
            _scoreService = new ScoreService(_context);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestSetScore()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand, DateTime.Today);

            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(1, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Single(weeklyScores.Scores.Where(s => s.Value == 1));
            var score = weeklyScores.Scores.FirstOrDefault(s => s.Value == 1);
            Assert.Equal(setScoreCommand.Date, score.Date);
            Assert.Equal(setScoreCommand.Task, score.Task);
        }

        [Fact]
        public async Task TestUnsetScore()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand, DateTime.Today);

            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task B",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            setScoreCommand.Value = 0;
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores.Where(s => s.Value == 1));
        }

        [Fact]
        public async Task TestSetScoreCaseInsensitive()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand, DateTime.Today);

            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task c",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(1, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Single(weeklyScores.Scores.Where(s => s.Value == 1));
            var score = weeklyScores.Scores.FirstOrDefault(s => s.Value == 1);
            Assert.Equal(setScoreCommand.Date, score.Date);
            Assert.Equal(setScoreCommand.Task, score.Task);
        }

        [Fact]
        public async Task TestSetScoreForInvalidTask()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand, DateTime.Today);

            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task D",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores.Where(s => s.Value == 1));
        }

    }
}