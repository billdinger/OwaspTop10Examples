using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class BlogEntryContext : DbContext
    {
        public BlogEntryContext (DbContextOptions<BlogEntryContext> options)
            : base(options)
        {
        }

        public DbSet<Blog.Models.BlogEntry> BlogEntry { get; set; }
    }
}
