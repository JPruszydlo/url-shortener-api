namespace ShortenerAPI.Entities
{
    public class UrlModel
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public string HostName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ToShort()
            => $"\"{HostName}/{ShortUrl}\"";

        public string ToLong()
            => LongUrl;
    }
}
