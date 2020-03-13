using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KidsPrize.Models
{
    public class ScoreResult
    {
        public Child Child { get; set; }

        public IEnumerable<WeeklyScores> WeeklyScores { get; set; }
    }

    public class WeeklyScores
    {
        [JsonConverter(typeof(DateConverter))]
        public DateTime Week { get; set; }

        public IEnumerable<string> Tasks { get; set; }

        public IEnumerable<Score> Scores { get; set; }
    }

    public class Score
    {
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }

        public string Task { get; set; }

        public int Value { get; set; }
    }
}