using EmoBot.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace EmoBot.Client.Controllers
{
    [Route("api/updates")]
    public class BotController : ControllerBase
    {
        private readonly IUpdateService _updateService;
        public BotController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpPost]
        public IActionResult Update([FromBody]Update update)
        {
            if (update == null)
                return Ok();

            _updateService.Update(update);

            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Bot is starting!");
        }
    }
}
