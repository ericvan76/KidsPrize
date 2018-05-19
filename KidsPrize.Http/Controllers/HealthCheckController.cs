using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KidsPrize.Http.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class HealthCheckController : VersionedController
    {
        private readonly KidsPrizeContext _context;
        private readonly HttpClient _httpClient;

        public HealthCheckController(KidsPrizeContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Get()
        {
            return NoContent();
        }
    }
}
