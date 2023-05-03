using Microsoft.EntityFrameworkCore;


namespace Filmovi_projekt.Models
{
    public class RatingContext : DbContext
    {

        public RatingContext(DbContextOptions<RatingContext> options) : base(options)
        {

        }
        public DbSet<Rating> Rating { get; set; }
    }
}