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
            modelBuilder.Entity<RequestMissingDetails>()
                .HasKey(rd => new { rd.DocumentPrefix, rd.DocumentSuffix, rd.ArticleCode });
            modelBuilder.Entity<RequestPreparation>()
                .HasKey(rp => new { rp.Code, rp.DocumentSuffix });
            modelBuilder.Entity<PurchaseOrderHeader>()
                .HasKey(rd => new { rd.OrderCode, rd.OrderPrefix, rd.OrderSuffix });
            modelBuilder.Entity<PurchaseOrderDetails>()
                .HasKey(rd => new { rd.OcCode, rd.SuffixOcCode, rd.PrefixOcCode, rd.ArticleCode });
        }

        public DbSet<Preparer> Preparers { get; set; }

        public DbSet<RequestHeaders> RequestHeaders { get; set; }

        public DbSet<RequestDetails> RequestDetails { get; set; }

        public DbSet<RequestPreparation> RequestPreparations { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<RequestMissingDetails> RequestMissingDetails { get; set; }

        public DbSet<PurchaseOrderHeader> PurchaseOrdersHeaders { get; set; }

        public DbSet<PurchaseOrderDetails> PurchaseOrderDetails { get; set; }
    }
}
