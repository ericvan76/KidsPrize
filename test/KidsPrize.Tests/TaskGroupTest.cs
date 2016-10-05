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
using E = KidsPrize.Models;

namespace KidsPrize.Tests
{
    public class TaskGroupTests
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

        public TaskGroupTests()
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
        public async Task TestUpdateTasks()
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
                Tasks = new[] { "Task B", "Task C", "Task E" }
            };
            TestHelper.ValidateModel(updateCommand);

            updateCommand.SetAuthorisation(_user);
            var actual = await _updateChildHandler.Handle(updateCommand);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(0, actual.Scores.Count());
            Assert.Equal(1, actual.TaskGroups.Count());
            Assert.Equal(DateTime.Today.StartOfWeek(), actual.TaskGroups.First().EffectiveDate);
            actual.TaskGroups.First().Tasks.SequenceEqual(updateCommand.Tasks);

            var allTaskGroups = await _context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Equal(1, allTaskGroups.Count);
        }

        [Fact]
        public async Task TestUpdateTasksAfterWeek()
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

            // mock taskGroup to previous week
            var child = await this._context.Children.FirstAsync(c => c.Id == createCommand.ChildId);
            var taskGroup = await this._context.TaskGroups.FirstAsync(tg => tg.Child.Id == createCommand.ChildId && tg.EffectiveDate == DateTime.Today.StartOfWeek());
            this._context.Remove(taskGroup);
            this._context.Add(new E.TaskGroup(child, DateTime.Today.AddDays(-7).StartOfWeek(), createCommand.Tasks));
            await this._context.SaveChangesAsync();

            var updateCommand = new UpdateChild()
            {
                ChildId = createCommand.ChildId,
                Tasks = new[] { "Task D", "Task C", "Task F" }
            };
            TestHelper.ValidateModel(updateCommand);

            updateCommand.SetAuthorisation(_user);
            var actual = await _updateChildHandler.Handle(updateCommand);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Equal(0, actual.Scores.Count());
            Assert.Equal(1, actual.TaskGroups.Count());
            Assert.Equal(DateTime.Today.StartOfWeek(), actual.TaskGroups.First().EffectiveDate);
            actual.TaskGroups.First().Tasks.SequenceEqual(updateCommand.Tasks);

            var allTaskGroups = await _context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == createCommand.ChildId).OrderBy(tg => tg.EffectiveDate).ToListAsync();
            Assert.Equal(2, allTaskGroups.Count);
            Assert.Equal(DateTime.Today.AddDays(-7).StartOfWeek(), allTaskGroups.First().EffectiveDate);
            allTaskGroups.First().Tasks.OrderBy(t=>t.Order).Select(t=>t.Name).SequenceEqual(updateCommand.Tasks);
        }

        [Fact]
        public async Task TestUpdateTasksWithScores()
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

            // mock taskGroup to previous week
            var child = await this._context.Children.FirstAsync(c => c.Id == createCommand.ChildId);
            var taskGroup = await this._context.TaskGroups.FirstAsync(tg => tg.Child.Id == createCommand.ChildId && tg.EffectiveDate == DateTime.Today.StartOfWeek());
            this._context.Remove(taskGroup);
            this._context.Add(new E.TaskGroup(child, DateTime.Today.AddDays(-7).StartOfWeek(), createCommand.Tasks));
            await this._context.SaveChangesAsync();

            // Task A for last week
            var setScoreCommand = new SetScore()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today.AddDays(-7),
                Task = "Task A",
                Value = 1
            };
            TestHelper.ValidateModel(setScoreCommand);

            setScoreCommand.SetAuthorisation(_user);
            await _setScoreHandler.Handle(setScoreCommand);

            // Task A for this week (to be cleaned)
            setScoreCommand.Task = "Task A";
            setScoreCommand.Date = DateTime.Today;
            await _setScoreHandler.Handle(setScoreCommand);

            // Task C for this week
            setScoreCommand.Task = "Task C";
            setScoreCommand.Date = DateTime.Today;
            await _setScoreHandler.Handle(setScoreCommand);

            // update tasks
            var updateCommand = new UpdateChild()
            {
                ChildId = createCommand.ChildId,
                Tasks = new[] { "Task D", "Task C", "Task F" }
            };
            TestHelper.ValidateModel(updateCommand);

            updateCommand.SetAuthorisation(_user);
            var actual = await _updateChildHandler.Handle(updateCommand);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(2, actual.Child.TotalScore);
            Assert.Equal(1, actual.Scores.Count());
            Assert.Equal("Task C", actual.Scores.First().Task);

            actual = await _scoreService.GetScores(_user.UserId(), createCommand.ChildId, DateTime.Today.StartOfWeek(), 1);
            Assert.Equal(2, actual.Child.TotalScore);
            Assert.Equal(1, actual.Scores.Count());
            Assert.Equal("Task A", actual.Scores.First().Task);
        }

    }
}
