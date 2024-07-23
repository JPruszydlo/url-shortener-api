using Microsoft.EntityFrameworkCore;

namespace ShortenerAPI.Entities
{
    public class ShortenerDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public DbSet<UrlModel> FreeUrls { get; set; }

        public ShortenerDbContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config["dbConnectionString"]);
        }
    }
}
