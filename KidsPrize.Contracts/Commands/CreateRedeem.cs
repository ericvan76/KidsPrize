using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Commands
{
    public class CreateRedeem
    {
        [Required]
        [JsonProperty("childId", Required = Required.Always)]
        public Guid ChildId { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }

        [Required]
        [JsonProperty("value", Required = Required.Always)]
        public int Value { get; set; }
    }
}