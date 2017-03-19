using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using R = KidsPrize.Resources;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    public class RedeemsController : ControllerWithMediator
    {
        private readonly IRedeemService _redeemService;

        public RedeemsController(IMediator mediator, IRedeemService redeemService) : base(mediator)
        {
            _redeemService = redeemService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(R.Redeem), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateRedeem([FromRoute] Guid childId, [FromBody] CreateRedeem command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            var result = await this.Send<CreateRedeem, R.Redeem>(command);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<R.Redeem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRedeems([FromRoute] Guid childId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var result = await this._redeemService.GetRedeems(this.User.UserId(), childId, limit, offset);
            return Ok(result);
        }
    }
}