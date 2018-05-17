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
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public HealthCheckController(KidsPrizeContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Get()
        {
            var pingUrl = _configuration.GetValue<string>("HealthCheckPingUrl");
            if (!string.IsNullOrEmpty(pingUrl))
            {
                var resp = await _httpClient.GetAsync(pingUrl);
                resp.EnsureSuccessStatusCode();
            }
            return NoContent();
        }
    }
}
