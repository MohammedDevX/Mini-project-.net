
using Microsoft.EntityFrameworkCore;
using Products_service.Data;
using Products_service.DTOS;
using Products_service.Models;

namespace Products_service.Repositories.Products
{
    public class ProduitR : IProduitR
    {
        private AppDbContext context;
        public ProduitR(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddProduct(Produit produit)
        {
            await context.Produit.AddAsync(produit);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductPrice(int id, ProductPriceDTO price)
        {
            await context.Produit.Where(p => p.Id == id)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.Prix, price.Price));
        }

        public async Task<double?> GetPrix(int id)
        {
            return await context.Produit.Where(p => p.Id == id).Select(p => p.Prix).SingleAsync();
        }

        public async Task<Produit?> GetProduit(int id)
        {
            return await context.Produit.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Produit>> GetProduits()
        {
            return await context.Produit.Include(c => c.Category).ToListAsync();
        }
    }
}
