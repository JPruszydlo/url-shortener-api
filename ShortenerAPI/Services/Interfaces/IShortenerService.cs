namespace ShortenerAPI.Services.Interfaces
{
    public interface IShortenerService
    {
        string GetLongUrlForShortUrl(string shortUrl);

        string ShortenUrl(string longUrl);
    }
}