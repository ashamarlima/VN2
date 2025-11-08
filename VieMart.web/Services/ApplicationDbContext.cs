using Microsoft.EntityFrameworkCore;
using VieMart.web.Models;  // <— important

namespace VieMart.web.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
    }
}
