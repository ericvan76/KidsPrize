using System.Threading.Tasks;

namespace KidsPrize.Bus
{
    public interface IHandleMessages<TCommand> where TCommand : Command
    {
        Task Handle(TCommand command);
        UserInfo User { get; set; }
    }
}