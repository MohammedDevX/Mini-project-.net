using Products_service.DTOS;
using Products_service.Models;

namespace Products_service.Repositories.Products
{
    public interface IProduitR
    {
        public Task AddProduct(Produit produit);
        public Task UpdateProductPrice(int id, ProductPriceDTO price);
        public Task<double?> GetPrix(int id);
        public Task<Produit?> GetProduit(int id);
        public Task<List<Produit>> GetProduits();
    }
}
