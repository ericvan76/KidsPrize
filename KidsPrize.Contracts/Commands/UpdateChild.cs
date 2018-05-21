using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Commands
{
    public class UpdateChild
    {
        [Required]
        [JsonProperty("childId", Required = Required.Always)]
        public Guid ChildId { get; set; }

        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [RegularExpression(@"^(M|F)$")]
        [JsonProperty("gender", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }

        [JsonProperty("tasks", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string[] Tasks { get; set; }
    }
}