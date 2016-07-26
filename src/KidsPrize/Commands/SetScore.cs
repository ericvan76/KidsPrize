using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Extensions;
using KidsPrize.Services;
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
            var child = await this._context.GetChildOrThrow(command.UserId(), command.ChildId);
            var day = await this._context.ResolveDay(child, command.Date, _defaultTasks);
            day.SetScore(command.Task, command.Value);
            await this._context.SaveChangesAsync();
        }
    }
}