using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Panier_service.DTOS;
using Panier_service.Models;
using Panier_service.ViewModels;

namespace Panier_service.Mappers
{
    public class PanierM
    {
        public static Panier ToPanier(PanierVM panier, PriceProductDTO price)
        {
            return new Panier
            {
                Id_client = panier.Id_client,
                Id_produit = panier.Id_produit,
                Qunatite = panier.Quantite,
                Prix = price.Prix * panier.Quantite
            };
        }

        public static PriceProductDTO ToPriceProductDTO(double? price)
        {
            return new PriceProductDTO
            {
                Prix = price
            };
        }

        public static QuantiteProductDTO ToQuantitePDTO(int quatite)
        {
            return new QuantiteProductDTO
            {
                Quantite = quatite
            };
        }
    }
}
