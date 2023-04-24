using Microsoft.EntityFrameworkCore;

namespace Filmovi.Models
{
    public class LoginContext:DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options):base(options) 
        { 
        
        }
        public DbSet<Login> Users { get; set; }
    }
}
