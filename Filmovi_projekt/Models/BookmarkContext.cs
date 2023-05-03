using Microsoft.EntityFrameworkCore;


namespace Filmovi_projekt.Models
{
    public class BookmarkContext : DbContext
    {
        
        public BookmarkContext(DbContextOptions<BookmarkContext> options) : base(options)
        {

        }
        public DbSet<Bookmark> Bookmark { get; set; }
    }
}
