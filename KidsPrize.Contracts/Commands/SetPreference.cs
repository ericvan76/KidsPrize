using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KidsPrize.Contracts.Commands
{
    public class SetPreference
    {
        [JsonProperty("timeZoneOffset", Required = Required.Default)]
        public int? TimeZoneOffset { get; set; }
    }
}