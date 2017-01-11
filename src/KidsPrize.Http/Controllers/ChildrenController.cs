using System;
using System.Collections.Generic;
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
    [Route("[controller]")]
    [Produces("application/json")]
    public class ChildrenController : ControllerWithMediator
    {
        private readonly IChildService _childService;

        public ChildrenController(IMediator mediator, IChildService childService) : base(mediator)
        {
            this._childService = childService;
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
            var result = await this.Send<CreateChild, ScoreResult>(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("{childId:guid}")]
        [ProducesResponseType(typeof(ScoreResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childId, [FromBody] UpdateChild command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            var result = await this.Send<UpdateChild, ScoreResult>(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{childId:guid}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childId)
        {
            await this.Send<DeleteChild>(new DeleteChild() { ChildId = childId });
            return StatusCode((int)HttpStatusCode.Accepted);
        }

    }
}