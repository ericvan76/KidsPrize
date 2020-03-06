using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KidsPrize.Http.Commands
{
    public class SetPreferenceCommand
    {
        [JsonProperty("timeZoneOffset", Required = Required.Default)]
        public int? TimeZoneOffset { get; set; }
    }
}