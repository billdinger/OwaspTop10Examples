using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class FeedContext : DbContext
    {
        public FeedContext(DbContextOptions<FeedContext> options)
            : base(options)
        {
        }

        public DbSet<Blog.Models.Feed> Feeds { get; set; }
    }
}
