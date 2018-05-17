using Xunit;
using KidsPrize.Services;
using AutoMapper;
using KidsPrize.Commands;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KidsPrize.Tests
{
    public class ChildTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly IScoreService _scoreService;
        private readonly string _userId;

        public ChildTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _scoreService = new ScoreService(_context, _mapper);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestCreateChild()
        {
            var command = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _childService.CreateChild(_userId, command);
            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, command.ChildId);

            Assert.Equal(command.Name, actual.Child.Name);
            Assert.Equal(command.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores);
            weeklyScores.Tasks.SequenceEqual(command.Tasks);
        }

        [Fact]
        public async Task TestUpdateChild()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _childService.CreateChild(_userId, createCommand);

            var updateCommand = new UpdateChild()
            {
                ChildId = createCommand.ChildId,
                Name = "New-Child-Name",
                Gender = "F"
            };

            await _childService.UpdateChild(_userId, updateCommand);
            var actual = await _scoreService.GetScoresOfCurrentWeek(_userId, createCommand.ChildId);

            Assert.Equal(updateCommand.Name, actual.Child.Name);
            Assert.Equal(updateCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores);
            weeklyScores.Tasks.SequenceEqual(createCommand.Tasks);
        }

        [Fact]
        public async Task TestDeleteChild()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _childService.CreateChild(_userId, createCommand);

            await _childService.DeleteChild(_userId, createCommand.ChildId);

            var child = await _context.Children.FirstOrDefaultAsync(c => c.Id == createCommand.ChildId);
            var scores = await _context.Scores.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            var taskgroups = await _context.TaskGroups.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Null(child);
            Assert.Empty(scores);
            Assert.Empty(taskgroups);
        }

        [Fact]
        public async Task TestDeleteChildWithScores()
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

            await _childService.DeleteChild(_userId, createCommand.ChildId);

            var child = await _context.Children.FirstOrDefaultAsync(c => c.Id == createCommand.ChildId);
            var scores = await _context.Scores.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            var taskgroups = await _context.TaskGroups.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Null(child);
            Assert.Empty(scores);
            Assert.Empty(taskgroups);
        }
    }
}
