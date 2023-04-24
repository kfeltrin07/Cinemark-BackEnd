using Microsoft.EntityFrameworkCore;

namespace Filmovi_projekt.Models
{
    public class CommentsContext : DbContext
    {

        public CommentsContext(DbContextOptions<CommentsContext> options) : base(options)
        {

        }
        public DbSet<Comments> Comments { get; set; }
    }
}
