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
        public Guid ChildUid { get; set; }
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
        private readonly KidsPrizeDbContext _context;
        private readonly DefaultTasks _defaultTasks;
        public UserInfo User { get; set; }

        public SetScoreHandler(KidsPrizeDbContext context, IOptions<DefaultTasks> defaultTasks)
        {
            this._context = context;
            this._defaultTasks = defaultTasks.Value;
        }
        public async Task Handle(SetScore command)
        {
            var userUid = User.Uid;
            var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
            var child = user.Children.FirstOrDefault(i => i.Uid == command.ChildUid);
            if (child == null)
            {
                throw new ArgumentException($"Child {command.ChildUid} not found.");
            }

            var day = await this._context.Days.Include(d => d.Child).Include(d => d.Scores)
                .FirstOrDefaultAsync(d => d.Child.Id == child.Id && d.Date == command.Date);

            if (day == null)
            {
                var childId = child.Id;
                var endOfWeek = command.Date.EndOfWeek();
                // get a close day from this/previous week
                var closeDay = await this._context.Days
                    .Where(d => d.Child.Id == childId && d.Date <= endOfWeek)
                    .OrderByDescending(d => d.Date).FirstOrDefaultAsync();
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