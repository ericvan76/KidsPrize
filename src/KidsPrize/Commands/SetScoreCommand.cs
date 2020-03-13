using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KidsPrize.Models;

namespace KidsPrize.Commands
{
    public class SetScoreCommand
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
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
}