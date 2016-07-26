using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;

namespace KidsPrize.Commands
{
    public class SetWeekTasks : Command
    {
        private DateTime _date;
        [Required]
        public Guid ChildId { get; set; }
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
        private readonly KidsPrizeContext _context;

        public SetWeekTasksHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        public async Task Handle(SetWeekTasks command)
        {
            var child = await this._context.GetChildOrThrow(command.UserId(), command.ChildId);
            var days = await this._context.ResolveRecentDays(child, command.Date);

            // Apply task for whole week
            var start = command.Date.StartOfWeek();
            for (int idx = 0; idx < 7; idx++)
            {
                var date = start.AddDays(idx);
                var day = days?.FirstOrDefault(d => d.Date == date);
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