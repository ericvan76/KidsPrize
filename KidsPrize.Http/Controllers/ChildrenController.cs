using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Abstractions;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class ChildrenController : VersionedController
    {
        private readonly IChildService _childService;
        private readonly IScoreService _scoreService;

        public ChildrenController(IChildService childService, IScoreService scoreService)
        {
            this._childService = childService;
            this._scoreService = scoreService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Child>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChildren()
        {
            var result = await this._childService.GetChildren(User.UserId());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateChild([FromBody] CreateChild command)
        {
            await this._childService.CreateChild(User.UserId(), command);
            var result = await this._scoreService.GetScoresOfCurrentWeek(User.UserId(), command.ChildId);
            return Ok(result);
        }

        [HttpPut("{childId:guid}")]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childId, [FromBody] UpdateChild command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            await this._childService.UpdateChild(User.UserId(), command);
            var result = await this._scoreService.GetScoresOfCurrentWeek(User.UserId(), command.ChildId);
            return Ok(result);
        }

        [HttpDelete("{childId:guid}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childId)
        {
            await this._childService.DeleteChild(User.UserId(), childId);
            return Ok();
        }

    }
}