using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Entities;

namespace KidsPrize.Services
{
    public interface IChildService
    {
        Task<Child> GetChild(string userId, Guid childId);
        Task<IEnumerable<Child>> GetChildren(string userId);
        Task<Child> CreateChild(string userId, CreateChild command);
        Task<Child> UpdateChild(string userId, UpdateChild command);
        Task DeleteChild(string userId, Guid childId);
    }

    public class ChildService : IChildService
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        public ChildService(KidsPrizeContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Child> CreateChild(string userId, CreateChild command)
        {
            var child = new E.Child(command.ChildId, userId, command.Name, command.Gender);
            var preference = await this._context.GetPreference(userId);

            var taskGroup = new E.TaskGroup(
                child,
                preference.Today().StartOfWeek(),
                command.Tasks);

            this._context.Children.Add(child);
            this._context.TaskGroups.Add(taskGroup);

            await _context.SaveChangesAsync();

            return _mapper.Map<Child>(child);
        }

        public async Task DeleteChild(string userId, Guid childId)
        {
            var child = await this._context.GetChildOrThrow(userId, childId);
            this._context.Children.Remove(child);

            await _context.SaveChangesAsync();
        }

        public async Task<Child> GetChild(string userId, Guid childId)
        {
            var child = await _context.GetChild(userId, childId);
            if (child != null)
            {
                return _mapper.Map<Child>(child);
            }
            return null;
        }

        public async Task<IEnumerable<Child>> GetChildren(string userId)
        {
            var result = new List<Child>();
            var children = await _context.Children.Where(i => i.UserId == userId).ToListAsync();
            children.ForEach(i => result.Add(_mapper.Map<Child>(i)));
            return result;
        }

        public async Task<Child> UpdateChild(string userId, UpdateChild command)
        {
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);
            child.Update(command.Name, command.Gender, null);

            var preference = await this._context.GetPreference(userId);

            if (command.Tasks != null && command.Tasks.Length > 0)
            {
                await UpdateTasks(child, command.Tasks, preference.Today().StartOfWeek());
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<Child>(child);
        }

        private async Task UpdateTasks(E.Child child, string[] tasks, DateTime effectiveDate)
        {
            var existingTaskgroup = await this._context.GetTaskGroup(child.Id, effectiveDate);

            if (!existingTaskgroup.Tasks.OrderBy(t => t.Order).Select(t => t.Name).SequenceEqual(tasks, StringComparer.OrdinalIgnoreCase))
            {
                if (existingTaskgroup.EffectiveDate == effectiveDate)
                {
                    existingTaskgroup.Update(tasks);
                }
                else
                {
                    var taskGroup = new E.TaskGroup(child, effectiveDate, tasks);
                    this._context.TaskGroups.Add(taskGroup);
                }

                // delete scores of removed Tasks
                var endDate = effectiveDate.AddDays(7);
                var removed = await this._context.Scores.Where(s =>
                    s.Child.Id == child.Id &&
                    s.Date >= effectiveDate &&
                    s.Date < endDate &&
                    !tasks.Contains(s.Task)
                ).ToListAsync();
                var delta = removed.Sum(s => s.Value);
                this._context.RemoveRange(removed);
                child.Update(null, null, child.TotalScore - delta);
            }
        }
    }
}