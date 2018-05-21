using System;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Models
{
    public class Redeem
    {
        [JsonProperty("timestamp", Required = Required.Always)]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public int Value { get; set; }
    }
}