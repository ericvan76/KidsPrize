using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Extensions
{
    public static class KidsPrizeContextExtensions
    {
        public static async Task<Child> GetChild(this KidsPrizeContext context, Guid userId, Guid childId)
        {
            return await context.Children.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == childId);
        }

        public static async Task<Child> GetChildOrThrow(this KidsPrizeContext context, Guid userId, Guid childId)
        {
            var child = await context.GetChild(userId, childId);
            if (child == null)
            {
                throw new ArgumentException($"Child {childId} not found.");
            }
            return child;
        }

        public static async Task<IEnumerable<Day>> ResolveRecentDays(this KidsPrizeContext context, Child child, DateTime date)
        {
            return await context.Days.Include(d => d.Child).Include(d => d.Scores)
                .Where(d => d.Child.Id == child.Id && d.Date <= date.EndOfWeek())
                .OrderByDescending(d => d.Date).Take(7)
                .ToListAsync();
        }

        public static async Task<Day> ResolveDay(this KidsPrizeContext context, Child child, DateTime date, DefaultTasks defaultTasks)
        {
            var days = await context.ResolveRecentDays(child, date);

            var day = days?.FirstOrDefault(d => d.Date == date);
            if (day == null)
            {
                var closeDay = days?.FirstOrDefault();
                var taskList = closeDay?.TaskList ?? defaultTasks;
                // create new day
                day = new Day(0, child, date, taskList.ToArray());
                context.Days.Add(day);
            }
            return day;
        }
    }
}