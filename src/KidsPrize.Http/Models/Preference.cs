using Newtonsoft.Json;

namespace KidsPrize.Http.Models
{
    public class Preference
    {
        [JsonProperty("timeZoneOffset", Required = Required.Always)]
        public int TimeZoneOffset { get; set; }
    }
}