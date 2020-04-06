using System.Net;
using System.Threading.Tasks;
using KidsPrize.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class HeartBeatController : ControllerBase
    {
        private readonly KidsPrizeContext _context;

        public HeartBeatController(KidsPrizeContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}