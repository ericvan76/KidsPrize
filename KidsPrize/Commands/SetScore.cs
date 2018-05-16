using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using KidsPrize.Extensions;
using Newtonsoft.Json;

namespace KidsPrize.Commands
{
    public class SetScore
    {
        [Required]
        [JsonProperty("childId", Required = Required.Always)]
        public Guid ChildId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        [JsonProperty("date", Required = Required.Always)]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonProperty("task", Required = Required.Always)]
        public string Task { get; set; }

        [Required]
        [Range(0, 1)]
        [JsonProperty("value", Required = Required.Always)]
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