using Microsoft.EntityFrameworkCore;
using VieMart.web.Models;

namespace VieMart.web.Migrations

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<ShortenedUrl>(e =>
            {
                e.Property(x => x.OriginalUrl).HasMaxLength(2048).IsRequired();
                e.Property(x => x.ShortCode).HasMaxLength(10).IsRequired();
                e.HasIndex(x => x.ShortCode).IsUnique();
            });
        }
    }
}
