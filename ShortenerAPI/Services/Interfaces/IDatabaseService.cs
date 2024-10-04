using ShortenerAPI.Entities;

namespace ShortenerAPI.Services.Interfaces
{
    public interface IDatabaseService
    {
        UrlModel? GetShortUrlForLongUrl(string longUrl);

        UrlModel? GetLongUrlForShortUrl(string shortUrl);

        bool ShortUrlExist(string shortUrl);

        UrlModel? SaveShortenResult(string shortUrl, string longUrl);
    }
}