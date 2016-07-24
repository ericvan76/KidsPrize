using System.Threading.Tasks;
using KidsPrize.Bus;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Bus
{
    public class ControllerWithBus : Controller
    {
        private readonly IBus _bus;

        public ControllerWithBus(IBus bus)
        {
            this._bus = bus;
        }

        public async Task Send<T>(T command) where T : Command
        {
            command.SetHeader("Authorisation", this.User);
            await _bus.Send(command);
        }

    }
}