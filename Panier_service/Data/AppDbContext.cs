using Microsoft.EntityFrameworkCore;
using Panier_service.Models;

namespace Panier_service.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Panier> Panier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Panier>().HasKey(p => new { p.Id_client, p.Id_produit });
        }
    }
}
