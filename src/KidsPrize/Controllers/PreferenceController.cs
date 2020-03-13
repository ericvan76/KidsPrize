
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Data;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PreferenceController : ControllerBase
    {
        private readonly KidsPrizeContext _context;

        public PreferenceController(KidsPrizeContext context)
        {
            this._context = context;
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetPreference([FromBody] SetPreferenceCommand command)
        {
            var preference = await this._context.Preferences.FirstOrDefaultAsync(p => p.UserId == User.UserId());
            if (preference == null)
            {
                preference = new Data.Entities.Preference(User.UserId(), 0);
                this._context.Preferences.Add(preference);
            }
            preference.Update(command.TimeZoneOffset);
            await this._context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Preference), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPreference()
        {
            var preference = await this._context.GetPreference(this.User.UserId());
            return Ok(new Preference { TimeZoneOffset = preference.TimeZoneOffset });
        }

    }
}