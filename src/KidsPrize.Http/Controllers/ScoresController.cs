using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Http.Bus;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    public class ScoresController : ControllerWithBus
    {
        private readonly IScoreService _scoreService;
        public ScoresController(IBus bus, IScoreService scoreService) : base(bus)
        {
            this._scoreService = scoreService;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> SetScore([FromRoute] Guid childId, [FromBody] SetScore command)
        {
            if (childId != command.ChildId)
            {
                return BadRequest("Unmatched ChildUid in route and command.");
            }
            await this.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }
    }
}