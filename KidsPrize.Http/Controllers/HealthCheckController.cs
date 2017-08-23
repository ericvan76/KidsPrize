using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class HealthCheckController : VersionedController
    {
        private readonly KidsPrizeContext _context;

        public HealthCheckController(KidsPrizeContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Get()
        {
            return NoContent();
        }
    }
}
