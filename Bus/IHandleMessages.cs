using System.Security.Claims;
using System.Threading.Tasks;

namespace KidsPrize.Bus
{
    public interface IHandleMessages<TCommand> where TCommand : Command
    {
        Task Handle(TCommand command);
        ClaimsPrincipal User { get; set; }
    }
}