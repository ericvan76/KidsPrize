using System;
using System.Collections.Generic;
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
    public class RedeemsController : VersionedController
    {
        private readonly IRedeemService _redeemService;

        public RedeemsController(IRedeemService redeemService)
        {
            _redeemService = redeemService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Redeem), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateRedeem([FromRoute] Guid childId, [FromBody] CreateRedeem command)
        {
            if (childId != command.ChildId)
            {
                ModelState.AddModelError(nameof(childId), "Unmatched childUid in route and command.");
                return BadRequest(ModelState);
            }
            var result = await this._redeemService.CreateRedeem(User.UserId(), command);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Redeem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRedeems([FromRoute] Guid childId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var result = await this._redeemService.GetRedeems(this.User.UserId(), childId, limit, offset);
            return Ok(result);
        }
    }
}