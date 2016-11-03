using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KidsPrize.Converters;
using Newtonsoft.Json;

namespace KidsPrize.Resources
{
    public class TaskGroup
    {
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateConverter))]
        public DateTime EffectiveDate { get; set; }
        public IEnumerable<string> Tasks { get; set; }
    }
}