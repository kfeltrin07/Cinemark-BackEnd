using Microsoft.EntityFrameworkCore;
using Filmovi_projekt.Models;

namespace Filmovi_projekt.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options):base(options) 
        { 
        
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
