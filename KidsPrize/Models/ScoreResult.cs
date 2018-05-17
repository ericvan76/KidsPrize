using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KidsPrize;
using Newtonsoft.Json;

namespace KidsPrize.Models
{
    public class ScoreResult
    {
        [JsonProperty("child", Required = Required.Always)]
        public Child Child { get; set; }

        [JsonProperty("weeklyScores", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<WeeklyScores> WeeklyScores { get; set; }
    }

    public class WeeklyScores
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        [JsonProperty("week", Required = Required.Always)]
        public DateTime Week { get; set; }

        [JsonProperty("tasks", Required = Required.Always)]
        public IEnumerable<string> Tasks { get; set; }

        [JsonProperty("scores", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Score> Scores { get; set; }
    }

    public class Score
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        [JsonProperty("date", Required = Required.Always)]
        public DateTime Date { get; set; }

        [JsonProperty("task", Required = Required.Always)]
        public string Task { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public int Value { get; set; }
    }
}