using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KidsPrize.Http.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class HealthCheckController : VersionedController
    {
        private readonly KidsPrizeContext _context;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly HttpClient _httpClient;

        public HealthCheckController(KidsPrizeContext context, IConfigurationRoot configurationRoot)
        {
            _context = context;
            _configurationRoot = configurationRoot;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Get()
        {
            var pingUrl = _configurationRoot.GetValue<string>("HealthCheckPingUrl");
            if (!string.IsNullOrEmpty(pingUrl))
            {
                var resp = await _httpClient.GetAsync(pingUrl);
                resp.EnsureSuccessStatusCode();
            }
            return NoContent();
        }
    }
}
