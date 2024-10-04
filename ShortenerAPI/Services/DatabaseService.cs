using ShortenerAPI.Entities;
using ShortenerAPI.Services.Interfaces;

namespace ShortenerAPI.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ShortenerDbContext _db;
        private readonly IConfiguration _config;
        public DatabaseService(ShortenerDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public UrlModel? GetLongUrlForShortUrl(string shortUrl)
            => _db.FreeUrls.FirstOrDefault(x => x.ShortUrl.ToLower() == shortUrl.ToLower());

        public UrlModel? GetShortUrlForLongUrl(string longUrl)
            => _db.FreeUrls.FirstOrDefault(x => x.LongUrl.ToLower() == longUrl.ToLower());

        public UrlModel? SaveShortenResult(string shortUrl, string longUrl)
        {
            try
            {
                var newUrlModel = new UrlModel()
                {
                    HostName = (_config["defaultHostName"] ?? "").ToLower(),
                    ShortUrl = shortUrl.ToLower(),
                    LongUrl = longUrl.ToLower(),
                    CreatedAt = DateTime.Now,
                };
                _db.FreeUrls.Add(newUrlModel);
                _db.SaveChanges();
                return newUrlModel;
            }
            catch
            {
                return null;
            }
        }

        public bool ShortUrlExist(string shortUrl)
            => _db.FreeUrls.FirstOrDefault(x => x.ShortUrl.ToLower() == shortUrl.ToLower()) != null;
    }
}
