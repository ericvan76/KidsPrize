using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Children/{childId:guid}/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly IChildService _service;

        public ScoresController(IChildService service)
        {
            this._service = service;
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetScore([FromRoute] Guid childId, [FromBody] SetScoreCommand command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            await this._service.SetScore(User.UserId(), command);
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
            var result = await this._service.GetScores(this.User.UserId(), childId, rewindFrom, numOfWeeks);
            return Ok(result);
        }
    }
}