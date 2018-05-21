using Newtonsoft.Json;

namespace KidsPrize.Contracts.Models
{
    public class Preference
    {
        [JsonProperty ("timeZoneOffset", Required = Required.Always)]
        public int TimeZoneOffset { get; set; }
    }
}