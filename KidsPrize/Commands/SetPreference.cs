using System.Threading.Tasks;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KidsPrize.Commands
{
    public class SetPreference
    {
        [JsonProperty("timeZoneOffset", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeZoneOffset { get; set; }
    }
}