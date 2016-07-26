using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsPrize.Commands
{
    public class SetScore : Command
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
        [MaxLength(50)]
        public string Task { get; set; }
        [Required]
        [Range(0, 1)]
        public int Value { get; set; }
    }

    public class SetScoreHandler : IHandleMessages<SetScore>
    {
        private readonly KidsPrizeContext _context;
        private readonly DefaultTasks _defaultTasks;

        public SetScoreHandler(KidsPrizeContext context, IOptions<DefaultTasks> defaultTasks)
        {
            this._context = context;
            this._defaultTasks = defaultTasks.Value;
        }
        public async Task Handle(SetScore command)
        {
            var child = await this._context.Children
                .FirstOrDefaultAsync(c => c.UserId == command.UserId() && c.Id == command.ChildId);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.ChildId} not found.");
            }
            var days = await this._context.Days.Include(d => d.Child).Include(d => d.Scores)
                .Where(d => d.Child.Id == child.Id && d.Date <= command.Date.EndOfWeek())
                .OrderByDescending(d => d.Date).Take(7)
                .ToListAsync();

            var day = days?.FirstOrDefault(d => d.Date == command.Date);
            if (day == null)
            {
                var closeDay = days?.FirstOrDefault();
                var taskList = closeDay?.TaskList ?? _defaultTasks;
                // create new day
                day = new Day(0, child, command.Date, taskList.ToArray());
                this._context.Days.Add(day);
            }
            day.SetScore(command.Task, command.Value);
            await this._context.SaveChangesAsync();
        }
    }
}