using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Http.Extensions;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class ChildrenController : Controller
    {
        private readonly IBus _bus;
        private readonly IChildService _childService;

        public ChildrenController(IBus bus, IChildService childService)
        {
            this._bus = bus;
            this._childService = childService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Child>))]
        public async Task<IActionResult> GetChildren()
        {
            var result = await this._childService.GetChildren(User.GetUserInfo().Uid);
            return Ok(result);
        }

        [HttpGet]
        [Route("{childUid:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Child))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetChild([FromRoute] Guid childUid)
        {
            var result = await this._childService.GetChild(User.GetUserInfo().Uid, childUid);
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
            await this._bus.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpPut]
        [Route("{childUid:guid}")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> UpdateChild([FromRoute] Guid childUid, [FromBody] UpdateChild command)
        {
            if (childUid != command.ChildUid)
            {
                return BadRequest("Unmatched ChildUid in route and command.");
            }
            await this._bus.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpDelete]
        [Route("{childUid:guid}")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> DeleteChild([FromRoute] Guid childUid)
        {
            await this._bus.Send(new DeleteChild() { ChildUid = childUid });
            return StatusCode((int)HttpStatusCode.Accepted);
        }


    }
}