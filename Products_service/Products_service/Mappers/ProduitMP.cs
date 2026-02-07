using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Products_service.DTOS;
using Products_service.Models;
using Shared.Contracts.Events;

namespace Products_service.Mappers
{
    public class ProduitMP
    {
        public static ProduitDTO ToProduitDTO(Produit produit, UserDTO user, string urlImage)
        {
            return new ProduitDTO
            {
                Id = produit.Id,
                Libelle = produit.Libelle,
                Prix = produit.Prix,
                Stock = produit.Stock,
                ImageName = urlImage,
                AdminId = produit.AdminId,
                NomAdmin = user.NomUser,
                CategoryId = produit.CategoryId,
                LibelleCategory = produit.Category.Libelle
            };
        }

        public static Produit ProduitVMToProduit(ProduitVM produit, string ImageName)
        {
            return new Produit
            {
                Libelle = produit.Libelle,
                Prix = produit.Prix,
                Stock = produit.Stock,
                ImageName = ImageName,
                AdminId = produit.AdminId,
                CategoryId = produit.CategoryId
            };
        }

        public static ProductUpdatedEvent ToUpdatedProductDTO(ProduitVM vm, int id)
        {
            return new ProductUpdatedEvent
            {
                ProductId = id,
                NewPrice = vm.Prix,
                UpdatedAt = DateTime.UtcNow
            };
        } 

        public static ProductPriceDTO ToProdPrice(ProduitVM vm)
        {
            return new ProductPriceDTO
            {
                Price = vm.Prix
            };
        }

        public static ProductImageDTO ToProdImage(IFormFile image)
        {
            return new ProductImageDTO
            {
                ImageName = image
            };
        }

        public static ProduitDTO ToProduitDtTO(Produit prod, string urlImage, string nomUser)
        {
            return new ProduitDTO
            {
                Id = prod.Id,
                Libelle = prod.Libelle,
                Prix = prod.Prix,
                Stock = prod.Stock,
                ImageName = urlImage,
                AdminId = prod.AdminId,
                NomAdmin = nomUser,
                CategoryId = prod.CategoryId,
                LibelleCategory = prod.Category.Libelle
            };
        }
    }
}
