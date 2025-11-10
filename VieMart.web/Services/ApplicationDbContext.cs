using Microsoft.EntityFrameworkCore;
using VieMart.web.Models;

namespace VieMart.web.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; } = null!; // keep if you use the shortener

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ---- Product mapping (Fluent API) ----
            b.Entity<Product>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Brand).HasMaxLength(100).IsRequired();
                e.Property(x => x.Category).HasMaxLength(50).IsRequired();

                e.Property(x => x.Price).HasPrecision(18, 2);    // decimal(18,2)
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.ImageFileName).HasMaxLength(255);

                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasIndex(x => x.Name);
            });

            // ---- ShortenedUrl mapping (if present) ----
            b.Entity<ShortenedUrl>(e =>
            {
                e.Property(x => x.OriginalUrl).HasMaxLength(2048).IsRequired();
                e.Property(x => x.ShortCode).HasMaxLength(10).IsRequired();
                e.HasIndex(x => x.ShortCode).IsUnique();
                e.Property(x => x.DateCreated).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
