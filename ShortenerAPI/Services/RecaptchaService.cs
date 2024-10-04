using ShortenerAPI.Models;
using ShortenerAPI.Services.Interfaces;

namespace ShortenerAPI.Services
{
    public class RecaptchaService : IRecaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;

        public RecaptchaService(IConfiguration configuration, HttpClient http)
        {
            _configuration = configuration;
            _http = http;
        }
        public async Task<bool> VerifyRecaptcha(string token)
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
                var response = await resp.Content.ReadFromJsonAsync<ReCaptchaResponse>();
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
    }
}