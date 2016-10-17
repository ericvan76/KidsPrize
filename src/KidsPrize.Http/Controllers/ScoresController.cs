using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Resources;
using KidsPrize.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    [Produces("application/json")]
    public class ScoresController : ControllerWithMediator
    {
        private readonly IScoreService _scoreService;

        public ScoresController(IMediator mediator, IScoreService scoreService) : base(mediator)
        {
            this._scoreService = scoreService;
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
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
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetScores([FromRoute] Guid childId, [FromQuery] DateTime rewindFrom, [FromQuery] int numOfWeeks)
        {
            if (!rewindFrom.IsCalendarDate())
            {
                return BadRequest("RewindFrom should be a calendar date.");
            }
            if (!rewindFrom.IsStartOfWeek())
            {
                return BadRequest("RewindFrom should be a calendar date.");
            }
            if (numOfWeeks <= 0)
            {
                return BadRequest("NumOfWeeks should be a positive value.");
            }
            var result = await this._scoreService.GetScores(this.User.UserId(), childId, rewindFrom, numOfWeeks);
            return Ok(result);
        }
    }
}