using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Controllers
{
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
            var result = await this.childService.GetChildren();
            return Ok(result);
        }

        [HttpGet]
        [Route("{childUid:guid}")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Child))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetChild([FromRoute] Guid childUid)
        {
            var result = await this.childService.GetChild(childUid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}