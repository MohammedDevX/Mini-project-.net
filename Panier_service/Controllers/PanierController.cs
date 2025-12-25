using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panier_service.Data;
using Panier_service.DTOS;
using Panier_service.Mappers;
using Panier_service.Models;
using Panier_service.Services;
using Panier_service.ViewModels;
using System.Security.Claims;

namespace Panier_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanierController : ControllerBase
    {
        private ProductService prod;
        private AppDbContext context;
        public PanierController(ProductService prod, AppDbContext context)
        {
            this.prod = prod;
            this.context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPanier(PanierVM vm) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existe = context.Panier.FirstOrDefault(p => p.Id_client == vm.Id_client && p.Id_produit == vm.Id_produit);

            if (existe != null)
            {
                QuantiteProductDTO quantite = PanierM.ToQuantitePDTO(vm.Quantite);
                existe.Prix += quantite.Quantite;
            } else
            {
                double? price = await prod.GetPriceProduct(vm.Id_produit);
                if (price == null)
                {
                    return BadRequest();
                }

                PriceProductDTO prix = PanierM.ToPriceProductDTO(price);
                Panier panier = PanierM.ToPanier(vm, prix);
                await context.Panier.AddAsync(panier);
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("get-panier/{idClient}")]
        public async Task<IActionResult> GetPanier(int idClient)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.Parse(userId) != idClient)
            {
                return Forbid();
            }

            List<Panier> panier = await context.Panier.Where(p => p.Id_client == idClient).ToListAsync();
            return Ok(panier);
        }
    }
}
