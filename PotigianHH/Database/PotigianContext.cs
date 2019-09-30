using Microsoft.EntityFrameworkCore;
using PotigianHH.Model;

namespace PotigianHH.Database
{
    public class PotigianContext : DbContext
    {
        public PotigianContext(
            DbContextOptions<PotigianContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestHeaders>()
                .HasKey(rh => new { rh.DocumentCode, rh.DocumentPrefix, rh.DocumentSuffix });
            modelBuilder.Entity<RequestDetails>()
                .HasKey(rd => new { rd.DocumentCode, rd.DocumentPrefix, rd.DocumentSuffix, rd.ArticleCode });
            modelBuilder.Entity<PurchaseOrder>()
                .HasKey(rd => new { rd.OrderCode, rd.OrderPrefix, rd.OrderSuffix });
        }

        public DbSet<Preparer> Preparers { get; set; }

        public DbSet<RequestHeaders> RequestHeaders { get; set; }

        public DbSet<RequestDetails> RequestDetails { get; set; }

        public DbSet<RequestPreparation> RequestPreparations { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
