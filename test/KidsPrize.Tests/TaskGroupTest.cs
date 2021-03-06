﻿using System;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Services;
using KidsPrize.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using E = KidsPrize.Data.Entities;

namespace KidsPrize.Tests
{
    public class TaskGroupTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IChildService _service;
        private readonly string _userId;

        public TaskGroupTests()
        {
            _context = TestHelper.CreateContext();
            _service = new ChildService(_context);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestUpdateTasks()
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
                Tasks = new[] { "Task B", "Task C", "Task E" }
            };
            await _service.UpdateChild(_userId, updateCommand, DateTime.Today);
            var actual = await _service.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores);
            weeklyScores.Tasks.SequenceEqual(updateCommand.Tasks);
            var allTaskGroups = await _context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == createCommand.ChildId).ToListAsync();
            Assert.Single(allTaskGroups);
        }

        [Fact]
        public async Task TestUpdateTasksAfterWeek()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            // mock taskGroup to previous week
            var child = await this._context.Children.FirstAsync(c => c.Id == createCommand.ChildId);
            var taskGroup = await this._context.TaskGroups.FirstAsync(tg => tg.Child.Id == createCommand.ChildId && tg.EffectiveDate == DateTime.Today.StartOfWeek());
            this._context.Remove(taskGroup);
            this._context.Add(new E.TaskGroup(child, DateTime.Today.AddDays(-7).StartOfWeek(), createCommand.Tasks));
            await this._context.SaveChangesAsync();

            var updateCommand = new UpdateChildCommand()
            {
                ChildId = createCommand.ChildId,
                Tasks = new[] { "Task D", "Task C", "Task F" }
            };
            await _service.UpdateChild(_userId, updateCommand, DateTime.Today);
            var actual = await _service.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(0, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Empty(weeklyScores.Scores);
            weeklyScores.Tasks.SequenceEqual(updateCommand.Tasks);

            var allTaskGroups = await _context.TaskGroups.Include(tg => tg.Tasks).Where(tg => tg.Child.Id == createCommand.ChildId).OrderBy(tg => tg.EffectiveDate).ToListAsync();
            Assert.Equal(2, allTaskGroups.Count);
            Assert.Equal(DateTime.Today.AddDays(-7).StartOfWeek(), allTaskGroups.First().EffectiveDate);
            allTaskGroups.First().Tasks.OrderBy(t => t.Order).Select(t => t.Name).SequenceEqual(updateCommand.Tasks);
        }

        [Fact]
        public async Task TestUpdateTasksWithScores()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "M",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };
            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            // mock taskGroup to previous week
            var child = await this._context.Children.FirstAsync(c => c.Id == createCommand.ChildId);
            var taskGroup = await this._context.TaskGroups.FirstAsync(tg => tg.Child.Id == createCommand.ChildId && tg.EffectiveDate == DateTime.Today.StartOfWeek());
            this._context.Remove(taskGroup);
            this._context.Add(new E.TaskGroup(child, DateTime.Today.AddDays(-7).StartOfWeek(), createCommand.Tasks));
            await this._context.SaveChangesAsync();

            // Task A for last week
            var setScoreCommand = new SetScoreCommand()
            {
                ChildId = createCommand.ChildId,
                Date = DateTime.Today.AddDays(-7),
                Task = "Task A",
                Value = 1
            };
            await _service.SetScore(_userId, setScoreCommand);

            // Task A for this week (to be cleaned)
            setScoreCommand.Task = "Task A";
            setScoreCommand.Date = DateTime.Today;
            await _service.SetScore(_userId, setScoreCommand);

            // Task C for this week
            setScoreCommand.Task = "Task C";
            setScoreCommand.Date = DateTime.Today;
            await _service.SetScore(_userId, setScoreCommand);

            // update tasks
            var updateCommand = new UpdateChildCommand()
            {
                ChildId = createCommand.ChildId,
                Tasks = new[] { "Task D", "Task C", "Task F" }
            };
            await _service.UpdateChild(_userId, updateCommand, DateTime.Today);
            var actual = await _service.GetScoresOfCurrentWeek(_userId, createCommand.ChildId, DateTime.Today);

            Assert.Equal(createCommand.Name, actual.Child.Name);
            Assert.Equal(createCommand.Gender, actual.Child.Gender);
            Assert.Equal(2, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            var weeklyScores = actual.WeeklyScores.First();
            Assert.Single(weeklyScores.Scores);
            Assert.Equal("Task C", weeklyScores.Scores.First().Task);

            actual = await _service.GetScores(_userId, createCommand.ChildId, DateTime.Today.StartOfWeek(), 1);
            Assert.Equal(2, actual.Child.TotalScore);
            Assert.Single(actual.WeeklyScores);
            weeklyScores = actual.WeeklyScores.First();
            Assert.Equal("Task A", weeklyScores.Scores.First().Task);
        }

    }
}