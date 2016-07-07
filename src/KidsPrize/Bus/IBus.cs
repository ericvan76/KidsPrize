using System.Threading.Tasks;

namespace KidsPrize.Bus
{
    public interface IBus
    {
        Task Send<T>(T command) where T : Command;
    }
}