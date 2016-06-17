using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityModel;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChildrenController : Controller
    {
        private readonly IChildService childService;

        public ChildrenController(IChildService childService)
        {
            this.childService = childService;
        }

        [HttpGet]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Child>))]
        public async Task<IActionResult> GetChildren()
        {
            var userUid = Guid.Parse(User.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value);
            var result = await this.childService.GetChildren(userUid);
            return Ok(result);
        }

        [HttpGet]
        [Route("{childUid:guid}")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Child))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetChild([FromRoute] Guid childUid)
        {
            var userUid = Guid.Parse(User.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value);
            var result = await this.childService.GetChild(userUid, childUid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}