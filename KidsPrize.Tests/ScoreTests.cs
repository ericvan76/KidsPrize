using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Services;
using Xunit;

namespace KidsPrize.Tests
{
    public class ScoreTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly IScoreService _scoreService;
        private readonly string _userId;

        public ScoreTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _scoreService = new ScoreService(_context, _mapper);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestSetScore()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId);

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
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task B",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            setScoreCommand.Value = 0;
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores.Where(s => s.Value == 1));
        }


        [Fact]
        public async Task TestSetScoreCaseInsensitive()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task c",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId);

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
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _childService.CreateChild(_userId, createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task D",
                Value = 1
            };
            await _scoreService.SetScore(_userId, setScoreCommand);

            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores.Where(s => s.Value == 1));
        }

    }
}