using System.Security.Claims;

namespace KidsPrize.Bus
{
    public class Command
    {
        public ClaimsPrincipal User { get; set; }
    }
}