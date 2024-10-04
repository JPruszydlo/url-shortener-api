using ShortenerAPI.Services.Interfaces;

namespace ShortenerAPI.Services
{
    public class ShortenenerService : IShortenerService
    {
        private readonly IDatabaseService _db;
        private readonly IConfiguration _config;
        public ShortenenerService(IDatabaseService db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string GetLongUrlForShortUrl(string shortUrl)
        {
            var url = _db.GetLongUrlForShortUrl(shortUrl);
            if (url != null)
            {
                return url.ToLong();
            }
            return _config["defaultHostName"] ?? "";
        }

        public string ShortenUrl(string longUrl)
        {
            var formattedLongUrl = new UriBuilder(longUrl).Uri.AbsoluteUri;

            var urlModel = _db.GetShortUrlForLongUrl(formattedLongUrl);
            if (urlModel != null)
            {
                return urlModel.ToShort();
            }

            string? shortUrl;
            do
            {
                shortUrl = RandomString();
            }
            while (_db.ShortUrlExist(shortUrl.ToLower()));

            var newUrlModel = _db.SaveShortenResult(shortUrl, formattedLongUrl);
            if (newUrlModel != null)
                return newUrlModel.ToShort();

            return string.Empty;
        }

        private static string RandomString()
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
}