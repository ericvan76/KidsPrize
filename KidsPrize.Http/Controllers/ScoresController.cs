using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    public class ScoresController : VersionedController
    {
        private readonly IScoreService _scoreService;

        public ScoresController(IScoreService scoreService)
        {
            this._scoreService = scoreService;
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetScore([FromRoute] Guid childId, [FromBody] SetScore command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            await this._scoreService.SetScore(User.UserId(), command);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetScores([FromRoute] Guid childId, [FromQuery] DateTime rewindFrom, [FromQuery] int numOfWeeks)
        {
            if (!rewindFrom.IsCalendarDate())
            {
                ModelState.AddModelError(nameof(rewindFrom), "rewindFrom should be a calendar date.");
            }
            if (!rewindFrom.IsStartOfWeek())
            {
                ModelState.AddModelError(nameof(rewindFrom), "rewindFrom should be a start of week.");
            }
            if (numOfWeeks <= 0)
            {
                ModelState.AddModelError(nameof(numOfWeeks), "numOfWeeks should be a positive value.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await this._scoreService.GetScores(this.User.UserId(), childId, rewindFrom, numOfWeeks);
            return Ok(result);
        }
    }
}