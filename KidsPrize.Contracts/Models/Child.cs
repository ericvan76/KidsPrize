using System;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Models
{
    public class Child
    {
        [JsonProperty ("id", Required = Required.Always)]
        public Guid Id { get; set; }

        [JsonProperty ("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty ("gender", Required = Required.Always)]
        public string Gender { get; set; }

        [JsonProperty ("totalScore", Required = Required.Always)]
        public int TotalScore { get; set; }
    }
}