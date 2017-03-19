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
        private readonly IScoreService _scoreService;
        private readonly CreateChildHandler _createChildHandler;
        private readonly SetScoreHandler _setScoreHandler;
        private readonly ClaimsPrincipal _user;

        public ScoreTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _scoreService = new ScoreService(_context, _mapper);
            _createChildHandler = new CreateChildHandler(_context, _scoreService);
            _setScoreHandler = new SetScoreHandler(_context);
            _user = TestHelper.CreateUser(_context);
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };
            TestHelper.ValidateModel(setScoreCommand);

            setScoreCommand.SetAuthorisation(_user);
            await _setScoreHandler.Handle(setScoreCommand);

            var actual = await _scoreService.GetScores(_user.UserId(), createCommand.ChildId, DateTime.Today.StartOfNextWeek(), 1);

            Assert.Equal(1, actual.Child.TotalScore);
            Assert.Equal(1, actual.WeeklyScores.Count());
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Equal(1, weeklyScores.Scores.Count(s => s.Value == 1));
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task B",
                Value = 1
            };
            TestHelper.ValidateModel(setScoreCommand);

            setScoreCommand.SetAuthorisation(_user);
            await _setScoreHandler.Handle(setScoreCommand);

            setScoreCommand.Value = 0;
            await _setScoreHandler.Handle(setScoreCommand);

            var actual = await _scoreService.GetScores(_user.UserId(), createCommand.ChildId, DateTime.Today.StartOfNextWeek(), 1);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(1, actual.WeeklyScores.Count());
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Equal(0, weeklyScores.Scores.Count(s => s.Value == 1));
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task c",
                Value = 1
            };
            TestHelper.ValidateModel(setScoreCommand);

            setScoreCommand.SetAuthorisation(_user);
            await _setScoreHandler.Handle(setScoreCommand);

            var actual = await _scoreService.GetScores(_user.UserId(), createCommand.ChildId, DateTime.Today.StartOfNextWeek(), 1);

            Assert.Equal(1, actual.Child.TotalScore);
            Assert.Equal(1, actual.WeeklyScores.Count());
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Equal(1, weeklyScores.Scores.Count(s => s.Value == 1));
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "task D",
                Value = 1
            };
            TestHelper.ValidateModel(setScoreCommand);

            setScoreCommand.SetAuthorisation(_user);
            await _setScoreHandler.Handle(setScoreCommand);

            var actual = await _scoreService.GetScores(_user.UserId(), createCommand.ChildId, DateTime.Today.StartOfNextWeek(), 1);

            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(1, actual.WeeklyScores.Count());
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Equal(0, weeklyScores.Scores.Count(s => s.Value == 1));
        }

    }
}