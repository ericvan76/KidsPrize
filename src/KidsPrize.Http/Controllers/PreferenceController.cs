
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using R = KidsPrize.Resources;

namespace KidsPrize.Http.Controllers
{
    [Route("[controller]")]
    public class PreferenceController : ControllerWithMediator
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;

        public PreferenceController(IMediator mediator, KidsPrizeContext context, IMapper mapper) : base(mediator)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> SetPreference([FromBody] SetPreference command)
        {
            await this.Send(command);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(R.Preference))]
        public async Task<IActionResult> GetPreference()
        {
            var preference = await this._context.GetPreference(this.User.UserId());
            return Ok(this._mapper.Map<R.Preference>(preference));
        }

    }
}