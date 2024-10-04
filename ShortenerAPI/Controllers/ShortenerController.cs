using Microsoft.AspNetCore.Mvc;
using ShortenerAPI.Services.Interfaces;

namespace ShortenerAPI.Controllers
{
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly IRecaptchaService _recaptchaService;
        private readonly IShortenerService _shortenerService;
        public ShortenerController(IRecaptchaService recaptchaService, IShortenerService shortenerService)
        {
            _recaptchaService = recaptchaService;
            _shortenerService = shortenerService;
        }

        [HttpGet("{shortUrl}")]
        public ActionResult RedirectToLongUrl([FromRoute] string shortUrl)
            => Redirect(_shortenerService.GetLongUrlForShortUrl(shortUrl));


        [HttpPost("[controller]/api")]
        public async Task<ActionResult> ShortenUrl([FromQuery] string longUrl, [FromBody] string token)
        {
            if (!await _recaptchaService.VerifyRecaptcha(token))
            {
                return Unauthorized();
            }

            var newUrl = _shortenerService.ShortenUrl(longUrl);
            return Ok(newUrl);
        }
    }
}
