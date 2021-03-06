using System;
using System.Collections.Generic;
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
    [Route("[controller]")]
    public class ChildrenController : ControllerBase
    {
        private readonly IChildService _service;

        public ChildrenController(IChildService service)
        {
            this._service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Child>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChildren()
        {
            var result = await this._service.GetChildren(User.UserId());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateChild([FromQuery] DateTime today, [FromBody] CreateChildCommand command)
        {
            await this._service.CreateChild(User.UserId(), command, today);
            var result = await this._service.GetScoresOfCurrentWeek(User.UserId(), command.ChildId, today);
            return Ok(result);
        }

        [HttpPut("{childId:guid}")]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childId, [FromQuery] DateTime today, [FromBody] UpdateChildCommand command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            await this._service.UpdateChild(User.UserId(), command, today);
            var result = await this._service.GetScoresOfCurrentWeek(User.UserId(), command.ChildId, today);
            return Ok(result);
        }

        [HttpDelete("{childId:guid}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childId)
        {
            await this._service.DeleteChild(User.UserId(), childId);
            return Ok();
        }

    }
}