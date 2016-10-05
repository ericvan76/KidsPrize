using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Resources;
using KidsPrize.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    public class ScoresController : ControllerWithMediator
    {
        private readonly IScoreService _scoreService;

        public ScoresController(IMediator mediator, IScoreService scoreService) : base(mediator)
        {
            this._scoreService = scoreService;
        }

        [HttpPut]
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

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type= typeof(ScoreResult))]
        public async Task<IActionResult> GetScores([FromRoute] Guid childId, [FromQuery] DateTime rewindFrom, [FromQuery] int numOfWeeks)
        {
            var result = await this._scoreService.GetScores(this.User.UserId(), childId, rewindFrom, numOfWeeks);
            return Ok(result);
        }
    }
}