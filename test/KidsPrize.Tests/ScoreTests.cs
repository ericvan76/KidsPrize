using System;
using System.Linq;
using AutoMapper;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Models;
using KidsPrize.Services;
using KidsPrize.Tests.Common;
using Microsoft.Extensions.Options;
using Xunit;

namespace KidsPrize.Tests
{
    public class ScoreTests
    {
        public static DefaultTasks defaultTasks = new DefaultTasks() { "Task A", "Task B", "Task C" };

        private readonly KidsPrizeDbContext _context;
        private readonly IMapper _mapper;
        private readonly IScoreService _scoreService;
        private readonly UserInfo _user;
        private readonly IBus _bus;


        public ScoreTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _scoreService = new ScoreService(_context, _mapper, new FakeDefaultTasksOptions());
            _user = TestHelper.CreateUser(_context);
            _bus = new TestBus(_user,
                new CreateChildHandler(_context),
                new SetScoreHandler(_context, new FakeDefaultTasksOptions()),
                new SetWeekTasksHandler(_context)
            );
        }

        public class FakeDefaultTasksOptions : IOptions<DefaultTasks>
        {
            public DefaultTasks Value { get; } = defaultTasks;
        }

        [Fact]
        public async void TestGetWeekScoresFromNew()
        {
            var command = new CreateChild()
            {
                ChildUid = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(command);

            var actual = await _scoreService.GetWeekScores(_user.Uid, command.ChildUid, DateTime.Today);

            Assert.Equal(command.ChildUid, actual.ChildUid);
            Assert.Equal(3, actual.Tasks.Count());
            Assert.Collection(actual.Tasks,
                s => Assert.Equal(defaultTasks[0], s),
                s => Assert.Equal(defaultTasks[1], s),
                s => Assert.Equal(defaultTasks[2], s));
            Assert.Equal(7, actual.DayScores.Count());
            Assert.All(actual.DayScores, i =>
            {
                Assert.Equal(3, i.Scores.Count());
                Assert.Collection(i.Scores,
                    s => Assert.Equal(defaultTasks[0], s.Task),
                    s => Assert.Equal(defaultTasks[1], s.Task),
                    s => Assert.Equal(defaultTasks[2], s.Task));
                Assert.All(i.Scores, v => Assert.Equal(0, v.Value));
            });
        }

        [Fact]
        public async void TestSetScore()
        {
            var childUid = Guid.NewGuid();
            var createChildCommand = new CreateChild()
            {
                ChildUid = childUid,
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(createChildCommand);

            var setScoreCommand = new SetScore()
            {
                ChildUid = childUid,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };
            await _bus.Send(setScoreCommand);
            var weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(1, weekScores.WeekTotal);
            Assert.Equal(1, weekScores.ChildTotal);

            setScoreCommand.Task = "Task B";
            await _bus.Send(setScoreCommand);
            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(2, weekScores.WeekTotal);
            Assert.Equal(2, weekScores.ChildTotal);

            setScoreCommand.Task = "Task C";
            setScoreCommand.Date = DateTime.Today.AddDays(7);
            await _bus.Send(setScoreCommand);
            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today.AddDays(7));
            Assert.Equal(1, weekScores.WeekTotal);
            Assert.Equal(3, weekScores.ChildTotal);

            setScoreCommand.Task = "Task B";
            setScoreCommand.Value = 0;
            setScoreCommand.Date = DateTime.Today;
            await _bus.Send(setScoreCommand);
            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(1, weekScores.WeekTotal);
            Assert.Equal(2, weekScores.ChildTotal);

            setScoreCommand.Task = "Dummy";
            await Assert.ThrowsAnyAsync<AggregateException>(async () => await _bus.Send(setScoreCommand));
            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(1, weekScores.WeekTotal);
            Assert.Equal(2, weekScores.ChildTotal);
        }

        [Fact]
        public async void TestSetTasks()
        {
            var childUid = Guid.NewGuid();
            var createChildCommand = new CreateChild()
            {
                ChildUid = childUid,
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(createChildCommand);

            var setScoreCommand = new SetScore()
            {
                ChildUid = childUid,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };
            await _bus.Send(setScoreCommand);
            var weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(1, weekScores.WeekTotal);

            setScoreCommand.Task = "Task B";
            await _bus.Send(setScoreCommand);
            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(2, weekScores.WeekTotal);

            var setTasksCommand = new SetWeekTasks()
            {
                ChildUid = childUid,
                Date = DateTime.Today,
                Tasks = new[] { "Task D", "Task C", "Task B" }
            };
            await _bus.Send(setTasksCommand);

            weekScores = await this._scoreService.GetWeekScores(_user.Uid, childUid, DateTime.Today);
            Assert.Equal(1, weekScores.WeekTotal);
            Assert.DoesNotContain("Task A", weekScores.Tasks);
            Assert.Collection(weekScores.Tasks,
                s => Assert.Equal("Task D", s),
                s => Assert.Equal("Task C", s),
                s => Assert.Equal("Task B", s));
        }
    }
}