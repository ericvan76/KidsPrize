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
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class ChildrenController : ControllerWithMediator
    {
        private readonly IChildService _childService;

        public ChildrenController(IMediator mediator, IChildService childService): base(mediator)
        {
            this._childService = childService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Child>))]
        public async Task<IActionResult> GetChildren()
        {
            var result = await this._childService.GetChildren(User.UserId());
            return Ok(result);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type= typeof(ScoreResult))]
        public async Task<IActionResult> CreateChild([FromBody] CreateChild command)
        {
            var result = await this.Send<CreateChild, ScoreResult>(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("{childId:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type= typeof(ScoreResult))]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childId, [FromBody] UpdateChild command)
        {
            if (childId != command.ChildId)
            {
                return BadRequest("Unmatched ChildUid in route and command.");
            }
            var result = await this.Send<UpdateChild, ScoreResult>(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{childId:guid}")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childId)
        {
            await this.Send<DeleteChild>(new DeleteChild() { ChildId = childId });
            return StatusCode((int)HttpStatusCode.Accepted);
        }

    }
}