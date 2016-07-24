using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Http.Bus;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class ChildrenController : ControllerWithBus
    {
        private readonly IChildService _childService;

        public ChildrenController(IBus bus, IChildService childService) : base(bus)
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

        [HttpGet]
        [Route("{childId:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Child))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetChild([FromRoute] Guid childId)
        {
            var result = await this._childService.GetChild(User.UserId(), childId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> CreateChild([FromBody] CreateChild command)
        {
            await this.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpPut]
        [Route("{childId:guid}")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childId, [FromBody] UpdateChild command)
        {
            if (childId != command.ChildId)
            {
                return BadRequest("Unmatched ChildUid in route and command.");
            }
            await this.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpDelete]
        [Route("{childId:guid}")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childId)
        {
            await this.Send(new DeleteChild() { ChildId = childId });
            return StatusCode((int)HttpStatusCode.Accepted);
        }


    }
}