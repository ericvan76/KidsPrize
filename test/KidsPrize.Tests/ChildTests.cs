using System;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Data;
using KidsPrize.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KidsPrize.Tests
{
    public class ChildTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IChildService _service;
        private readonly string _userId;

        public ChildTests()
        {
            _context = TestHelper.CreateContext();
            _service = new ChildService(_context);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestCreateChild()
        {
            var command = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _service.CreateChild(_userId, command, DateTime.Today);
            var actual = await _service.GetScoresOfCurrentWeek(_userId, command.ChildId, DateTime.Today);

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
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            var updateCommand = new UpdateChildCommand()
            {
                ChildId = createCommand.ChildId,
                Name = "New-Child-Name",
                Gender = "F"
            };

            await _service.UpdateChild(_userId, updateCommand, DateTime.Today);
            var actual = await _service.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

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
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            await _service.DeleteChild(_userId, createCommand.ChildId);

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
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today,
                Task = "Task A",
                Value = 1
            };

            await _service.SetScore(_userId, setScoreCommand);

            await _service.DeleteChild(_userId, createCommand.ChildId);

            var child = await _context.Children.FirstOrDefaultAsync(c => c.Id == createCommand.ChildId);
            var scores = await _context.Scores.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            var taskgroups = await _context.TaskGroups.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Null(child);
            Assert.Empty(scores);
            Assert.Empty(taskgroups);
        }
    }
}