
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Http.Commands;
using KidsPrize.Http.Models;
using KidsPrize.Repository.Npgsql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Http.Controllers
{
    [Route("v2/[controller]")]
    public class PreferenceController : Controller
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;

        public PreferenceController(KidsPrizeContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetPreference([FromBody] SetPreferenceCommand command)
        {
            var preference = await this._context.Preferences.FirstOrDefaultAsync(p => p.UserId == User.UserId());
            if (preference == null)
            {
                preference = new Repository.Npgsql.Entities.Preference(User.UserId(), 0);
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
            return Ok(this._mapper.Map<Preference>(preference));
        }

    }
}