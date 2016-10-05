using Xunit;
using KidsPrize.Services;
using AutoMapper;
using KidsPrize.Commands;
using System;
using System.Security.Claims;
using KidsPrize.Extensions;
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
        private readonly CreateChildHandler _createChildHandler;
        private readonly UpdateChildHandler _updateChildHandler;
        private readonly DeleteChildHandler _deleteChildHandler;
        private readonly SetScoreHandler _setScoreHandler;
        private readonly ClaimsPrincipal _user;

        public ChildTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _scoreService = new ScoreService(_context, _mapper);
            _createChildHandler = new CreateChildHandler(_context, _scoreService);
            _updateChildHandler = new UpdateChildHandler(_context, _scoreService);
            _deleteChildHandler = new DeleteChildHandler(_context);
            _setScoreHandler = new SetScoreHandler(_context);
            _user = TestHelper.CreateUser(_context);
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

            TestHelper.ValidateModel(command);

            command.SetAuthorisation(_user);
            var actual = await _createChildHandler.Handle(command);

            Assert.Equal(command.Name, actual.Child.Name);
            Assert.Equal(command.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(0, actual.Scores.Count());
            Assert.Equal(1, actual.TaskGroups.Count());
            Assert.Equal(DateTime.Today.StartOfWeek(), actual.TaskGroups.First().EffectiveDate);
            actual.TaskGroups.First().Tasks.SequenceEqual(command.Tasks);
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var updateCommand = new UpdateChild()
            {
                ChildId = createCommand.ChildId,
                Name = "New-Child-Name",
                Gender = "F"
            };
            TestHelper.ValidateModel(updateCommand);

            updateCommand.SetAuthorisation(_user);
            var actual = await _updateChildHandler.Handle(updateCommand);

            Assert.Equal(updateCommand.Name, actual.Child.Name);
            Assert.Equal(updateCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(0, actual.Scores.Count());
            Assert.Equal(1, actual.TaskGroups.Count());
            Assert.Equal(DateTime.Today.StartOfWeek(), actual.TaskGroups.First().EffectiveDate);
            actual.TaskGroups.First().Tasks.SequenceEqual(createCommand.Tasks);
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
            TestHelper.ValidateModel(createCommand);

            createCommand.SetAuthorisation(_user);
            await _createChildHandler.Handle(createCommand);

            var deleteCommand = new DeleteChild()
            {
                ChildId = createCommand.ChildId
            };
            TestHelper.ValidateModel(deleteCommand);

            deleteCommand.SetAuthorisation(_user);
            await _deleteChildHandler.Handle(deleteCommand);

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

            var deleteCommand = new DeleteChild()
            {
                ChildId = createCommand.ChildId
            };
            TestHelper.ValidateModel(deleteCommand);

            deleteCommand.SetAuthorisation(_user);
            await _deleteChildHandler.Handle(deleteCommand);

            var child = await _context.Children.FirstOrDefaultAsync(c => c.Id == createCommand.ChildId);
            var scores = await _context.Scores.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            var taskgroups = await _context.TaskGroups.Where(c => c.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Null(child);
            Assert.Empty(scores);
            Assert.Empty(taskgroups);
        }
    }
}
