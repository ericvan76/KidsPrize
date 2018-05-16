using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KidsPrize.Models
{
    public class Redeem
    {
        [DataType(DataType.DateTime)]
        [JsonProperty("timestamp", Required = Required.Always)]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public int Value { get; set; }
    }
}