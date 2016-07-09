using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childUid:guid}/[controller]")]
    public class ScoresController : Controller
    {
        private readonly IBus _bus;
        private readonly IScoreService _scoreService;
        public ScoresController(IBus bus, IScoreService scoreService)
        {
            this._bus = bus;
            this._scoreService = scoreService;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> SetScore([FromRoute] Guid childUid, [FromBody] SetScore command)
        {
            if (childUid != command.ChildUid)
            {
                return BadRequest("Unmatched ChildUid in route and command.");
            }
            await this._bus.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }
    }
}