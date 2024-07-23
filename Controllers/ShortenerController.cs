using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShortenerAPI.Entities;
using System.Net.Http;

namespace ShortenerAPI.Controllers
{
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly ShortenerDbContext _context;
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;
        public ShortenerController(ShortenerDbContext context, HttpClient http, IConfiguration config)
        {
            _context = context;
            _http = http;
            _configuration = config;
        }

        [HttpGet("{shortUrl}")]
        public ActionResult GetShortUrl([FromRoute] string shortUrl)
        {
            var url = _context.FreeUrls.FirstOrDefault(x => x.ShortUrl.ToLower() == shortUrl.ToLower());
            if (url != null)
            {
                return Redirect(url.ToLong());
            }
            return Redirect(_configuration["defaultHostName"] ?? "");
        }

        [HttpPost("[controller]/api")]
        public async Task<ActionResult> ShortenUrl([FromQuery] string longUrl, [FromBody] string token)
        {
            if (!await VerifyReCaptcha(token))
            {
                return BadRequest();
            }

            var formattedLongUrl = new UriBuilder(longUrl).Uri.AbsoluteUri;

            var urlModel = _context.FreeUrls.FirstOrDefault(x => x.LongUrl.ToLower() == formattedLongUrl.ToLower());
            if (urlModel != null)
            {
                return Ok($"\"{urlModel.ToShort()}\"");
            }

            var shortUrl = string.Empty;
            do
            {
                shortUrl = RandomString();
            }
            while (_context.FreeUrls.FirstOrDefault(x => x.ShortUrl.ToLower() == shortUrl.ToLower()) != null);

            var newUrlModel = new UrlModel()
            {
                HostName = (_configuration["defaultHostName"] ?? "").ToLower(),
                ShortUrl = shortUrl.ToLower(),
                LongUrl = formattedLongUrl.ToLower(),
                CreatedAt = DateTime.Now,
            };
            _context.FreeUrls.Add(newUrlModel);
            _context.SaveChanges();

            return Ok($"\"{newUrlModel.ToShort()}\"");
        }

        private async Task<bool> VerifyReCaptcha(string token)
        {
            var secretKey = _configuration["reCaptcha:secretCode"];
            if (string.IsNullOrEmpty(token))
                return false;

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"secret", secretKey},
                {"response", token }
            });

            var resp = await _http.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadFromJsonAsync<reCaptchaResponse>();
                var results = new List<bool>()
                {
                    response.Success,
                    response.Score > 0.5,
                    response.HostName == _configuration["reCaptcha:hostName"],
                    response.Action == "shortenurl"
                };
                return results.All(x => x);
            }
            return false;
        }

        public static string RandomString()
        {
            var chars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 6)
                            .Select(s => s[random.Next(s.Length)])
                            .ToArray());

            return result;
        }
    }
    public class reCaptchaResponse
    {
        public bool Success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string apk_package_name { get; set; }
        public string HostName { get; set; }
        public string[] ErrorCodes { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
        public override string ToString()
        {
            string result = "reCaptcehaResponse\n";
            result += $"Success={Success}\n";
            result += $"ErrorCodes={ErrorCodes}\n";
            result += $"Score={Score}\n";
            result += $"Action={Action}\n";
            result += $"challenge_ts={challenge_ts}\n";
            result += $"apk_package_name={apk_package_name}\n";
            result += $"HostName={HostName}\n";
            return result;
        }
    }
}
