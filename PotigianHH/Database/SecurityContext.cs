using Microsoft.EntityFrameworkCore;
using PotigianHH.Model;

namespace PotigianHH.Database
{
    public class SecurityContext : DbContext
    {
        public SecurityContext(
            DbContextOptions<SecurityContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AccessSystem> AccessSystems { get; set; }
    }
}
