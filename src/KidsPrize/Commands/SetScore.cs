using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Converters;
using KidsPrize.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using E = KidsPrize.Models;

namespace KidsPrize.Commands
{
    public class SetScore : Command, IAsyncRequest, IValidatableObject
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(50)]
        public string Task { get; set; }

        [Required]
        [Range(0, 1)]
        public int Value { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (!Date.IsCalendarDate())
            {
                results.Add(new ValidationResult("Date should be a calendar date.", new[] { nameof(Date) }));
            }
            return results;
        }
    }

    public class SetScoreHandler : AsyncRequestHandler<SetScore>
    {
        private readonly KidsPrizeContext _context;

        public SetScoreHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        protected override async Task HandleCore(SetScore message)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(message.UserId(), message.ChildId);

            // Validate Task
            var taskGroup = await this._context.GetTaskGroup(child.Id, message.Date);
            if (taskGroup == null || !taskGroup.Tasks.Any(t => t.Name.Equals(message.Task, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var score = this._context.Scores.Include(s => s.Child)
                .FirstOrDefault(s => s.Child.Id == child.Id && s.Date == message.Date && s.Task == message.Task);

            if (score == null)
            {
                score = new E.Score(child, message.Date, message.Task, 0);
                this._context.Scores.Add(score);
            }

            score.Update(message.Value);

            await this._context.SaveChangesAsync();
        }
    }
}