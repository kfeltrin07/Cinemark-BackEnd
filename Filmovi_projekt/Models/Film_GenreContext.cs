using Microsoft.EntityFrameworkCore;

namespace Filmovi_projekt.Models
{
    public class Film_GenreContext:DbContext
    {
        public Film_GenreContext(DbContextOptions<Film_GenreContext> options) : base(options) 
        { 
        
        }
        public DbSet<Film_Genre> Film_Genre { get; set; }
    }
}
