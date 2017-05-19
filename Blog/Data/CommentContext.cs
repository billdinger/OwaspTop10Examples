using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class CommentContext : DbContext
    {
        public CommentContext (DbContextOptions<CommentContext> options)
            : base(options)
        {
        }

        public DbSet<Blog.Models.Comment> Comment { get; set; }
    }
}
