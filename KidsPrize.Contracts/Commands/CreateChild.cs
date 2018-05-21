using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Commands
{
    public class CreateChild
    {
        [Required]
        [JsonProperty ("childId", Required = Required.Always)]
        public Guid ChildId { get; set; }

        [Required]
        [JsonProperty ("name", Required = Required.Always)]
        public string Name { get; set; }

        [Required]
        [RegularExpression (@"^(M|F)$")]
        [JsonProperty ("gender", Required = Required.Always)]
        public string Gender { get; set; }

        [Required]
        [MinLength (1)]
        [JsonProperty ("tasks", Required = Required.Always)]
        public string[] Tasks { get; set; }

    }
}