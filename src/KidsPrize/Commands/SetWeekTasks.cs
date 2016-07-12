using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class SetWeekTasks : Command
    {
        private DateTime _date;
        [Required]
        public Guid ChildUid { get; set; }
        [Required]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value.Date; }
        }
        [Required]
        public string[] Tasks { get; set; }

    }

    public class SetWeekTasksHandler : IHandleMessages<SetWeekTasks>
    {
        private readonly KidsPrizeDbContext _context;
        public UserInfo User { get; set; }

        public SetWeekTasksHandler(KidsPrizeDbContext context)
        {
            this._context = context;
        }

        public async Task Handle(SetWeekTasks command)
        {
            var userUid = User.Uid;
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            var child = user.Children.FirstOrDefault(i => i.Uid == command.ChildUid);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.ChildUid} not found.");
            }

            // Apply task for whole week
            var start = command.Date.StartOfWeek();
            for (int idx = 0; idx < 7; idx++)
            {
                var date = start.AddDays(idx);
                var day = await this._context.Days.Include(d => d.Child).Include(d => d.Scores)
                    .FirstOrDefaultAsync(d => d.Child.Id == child.Id && d.Date == date);
                if (day != null)
                {
                    day.SetTasks(command.Tasks);
                }
                else
                {
                    day = new Day(0, child, date, command.Tasks);
                    this._context.Days.Add(day);
                }
            }
            await this._context.SaveChangesAsync();
        }
    }
}