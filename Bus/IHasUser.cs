using System.Security.Claims;

namespace KidsPrize.Bus
{
    public interface IHasUser
    {
        ClaimsPrincipal User { get; set; }
    }
}