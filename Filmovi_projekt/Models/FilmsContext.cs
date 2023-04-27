using Microsoft.EntityFrameworkCore;

namespace Filmovi_projekt.Models
{
    public class FilmsContext : DbContext
    {
        public FilmsContext(DbContextOptions<FilmsContext> options) : base(options)
        {

        }
        public DbSet<Films> Films { get; set; }
    }
}
