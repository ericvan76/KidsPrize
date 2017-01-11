using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KidsPrize.Converters;
using Newtonsoft.Json;

namespace KidsPrize.Resources
{
    public class ScoreResult
    {
        public Child Child { get; set; }
        public IEnumerable<WeeklyScores> WeeklyScores { get; set; }
    }

    public class WeeklyScores
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        public DateTime Week { get; set; }

        public IEnumerable<string> Tasks { get; set; }
        public IEnumerable<Score> Scores { get; set; }
    }

    public class Score
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public int Value { get; set; }
    }
}