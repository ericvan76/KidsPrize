using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Extensions;
using KidsPrize.Http.Bus;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childId:guid}/[controller]")]
    public class WeeksController : ControllerWithBus
    {
        private readonly IScoreService _scoreService;

        public WeeksController(IBus bus, IScoreService scoreService) : base(bus)
        {
            this._scoreService = scoreService;
        }

        [HttpPost]
        [Route("{date:datetime}/Tasks")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> SetTasks([FromRoute] Guid childId, [FromRoute] DateTime date, [FromBody] string[] tasks)
        {
            await this.Send(new SetWeekTasks() { ChildId = childId, Date = date.Date, Tasks = tasks });
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpGet]
        [Route("{date:datetime}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(WeekScores))]
        public async Task<IActionResult> GetScores([FromRoute] Guid childId, [FromRoute] DateTime date)
        {
            var result = await this._scoreService.GetWeekScores(User.UserId(), childId, date.Date);
            return Ok(result);
        }
    }
}