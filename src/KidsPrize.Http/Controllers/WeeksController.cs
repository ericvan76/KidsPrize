using System;
using System.Net;
using System.Threading.Tasks;
using KidsPrize.Bus;
using KidsPrize.Commands;
using KidsPrize.Http.Extensions;
using KidsPrize.Resources;
using KidsPrize.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace KidsPrize.Http.Controllers
{
    [Route("Children/{childUid:guid}/[controller]")]
    public class WeeksController : Controller
    {
        private readonly IBus _bus;
        private readonly IScoreService _scoreService;

        public WeeksController(IBus bus, IScoreService scoreService)
        {
            this._bus = bus;
            this._scoreService = scoreService;
        }

        [HttpPost]
        [Route("{date:datetime}/Tasks")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IActionResult> SetTasks([FromRoute] Guid childUid, [FromRoute] DateTime date, [FromBody] string[] tasks)
        {
            await this._bus.Send(new SetWeekTasks() { ChildUid = childUid, Date = date, Tasks = tasks });
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpGet]
        [Route("{date:datetime}/Scores")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(WeekScores))]
        public async Task<IActionResult> GetScores([FromRoute] Guid childUid, [FromRoute] DateTime date)
        {
            var result = await this._scoreService.GetWeekScores(User.GetUserInfo().Uid, childUid, date);
            return Ok(result);
        }
    }
}