using System;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Contracts;
using KidsPrize.Repository.Npgsql.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Repository.Npgsql
{
    public static class KidsPrizeContextExtensions
    {
        public static async Task<Child> GetChild(this KidsPrizeContext context, string userId, Guid childId)
        {
            return await context.Children.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == childId);
        }

        public static async Task<Child> GetChildOrThrow(this KidsPrizeContext context, string userId, Guid childId)
        {
            var child = await context.GetChild(userId, childId);
            if (child == null)
            {
                throw new ArgumentException($"Child {childId} not found.");
            }
            return child;
        }

        public static async Task<TaskGroup> GetTaskGroup(this KidsPrizeContext context, Guid childId, DateTime calendarDate)
        {
            var effectiveDate = calendarDate.StartOfWeek();
            return await context.TaskGroups
                .Include(tg => tg.Tasks)
                .Where(tg => tg.Child.Id == childId && tg.EffectiveDate <= effectiveDate)
                .OrderByDescending(tg => tg.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public static async Task<Preference> GetPreference(this KidsPrizeContext context, string userId)
        {
            return await context.Preferences.FirstOrDefaultAsync(p => p.UserId == userId)
                ?? new Preference(userId, 0);
        }
    }
}