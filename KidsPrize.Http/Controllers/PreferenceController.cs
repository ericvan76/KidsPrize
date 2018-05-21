
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;
using KidsPrize.Repository.Npgsql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class PreferenceController : VersionedController
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
        public async Task<IActionResult> SetPreference([FromBody] SetPreference command)
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