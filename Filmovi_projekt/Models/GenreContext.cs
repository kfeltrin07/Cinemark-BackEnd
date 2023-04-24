using Microsoft.EntityFrameworkCore;

namespace Filmovi_projekt.Models
{
    public class GenreContext:DbContext
    {
        public GenreContext(DbContextOptions<GenreContext> options) : base(options) 
        {
            
        }

        public DbSet<Genre> Genres { get; set; }
    }
}
