using System;
using System.ComponentModel.DataAnnotations;
using KidsPrize.Converters;
using Newtonsoft.Json;

namespace KidsPrize.Resources
{
    public class Score
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }
        public string Task { get; set; }
        public int Value { get; set; }
    }
}