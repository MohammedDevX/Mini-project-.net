using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_service.Data;
using Products_service.DTOS;
using Products_service.Mappers;
using Products_service.Models;
using Products_service.Repositories.Category;
using Products_service.Repositories.Products;
using Products_service.Services;
using Products_service.Services.Caching;
using Products_service.Services.Kafka;
using Products_service.Services.SaveFiles;

namespace Products_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private AppDbContext context;
        private UserService userAC;
        private IRedisCacheService cache;
        private IMinioService minio;
        private ICategoryR categoryR;
        private IProduitR produitR;
        public CatalogueController(
            AppDbContext context, UserService userAC, IRedisCacheService cache, IMinioService minio, ICategoryR category,
            IProduitR produit
            )
        {
            this.context = context;
            this.userAC = userAC;
            this.cache = cache;
            this.minio = minio;
            categoryR = category;
            produitR = produit;
        }

        [HttpGet("price/{id}")]
        public async Task<ActionResult<double>> GetPrice(int id)
        {
            double? price = await produitR.GetPrix(id);
            if (price == null)
            {
                return NotFound();
            }
            return Ok(price);
        }

        [HttpGet("index/{id}")]
        public async Task<ActionResult<Produit>> Index(int id)
        {
            Produit? product = await produitR.GetProduit(id);
            if (product == null)
            {
                return NotFound();
            }

            var urlImage = await minio.GetImage(product.ImageName, 3600);

            UserDTO? user = await userAC.GetUserAsync(product.AdminId);
            ProduitDTO produitD = ProduitMP.ToProduitDTO(product, user, urlImage);
            return Ok(produitD);
        }

        [HttpGet("index")]
        public async Task<ActionResult<List<Produit>>> Index()
        {
            // 1) Si pas de donnes personalises, comme notre cas touts les users vont avoire la meme liste de prods
            //string cachingKey = "catalogue";

            // 2) Si Vous voulez cachez des donnes personalises par ex : afficher pour chaque user les listes 
            // des produits favoris
            // donc dans le frontend quand on fecth cette action en doit envoyer dans Headers le UserId

            // N.B : Le flux est que la requete contient le userId dans le hraders, donc on doit recuperer les produits 
            // dans ce user à creer et casher ces donnes filtres par UserId chaque listes de donnes est signer par 
            // un key dans le cash par ex catalogue, si on ajouter le userId et on fait catalogue:1 par ex 
            // on doit stocker les produits creer par user 1
            var userId = Request.Headers["UserId"]; // Cette ligne permet de lire d'apres headers d'une requete 
            // speseficement le UserId / N.B :changer par token
            var cachingKey = $"catalogue:{userId}";
            var cachedProducts = await cache.GetData<List<ProduitDTO>>(cachingKey);
            if (cachedProducts != null)
            {
                return Ok(cachedProducts);
            }

            List<Produit> products = await produitR.GetProduits();

            var produits = new List<ProduitDTO>();

            foreach (var prod in products)
            {
                var urlImage = await minio.GetImage(prod.ImageName, 3600);
                var user = await userAC.GetUserAsync(prod.AdminId);
                if (user == null)
                {
                    return StatusCode(502, "Error user service!");
                }
                produits.Add(ProduitMP.ToProduitDtTO(prod, urlImage, user.NomUser));
            }

            await cache.SetData(cachingKey, produits);
            return Ok(produits);
        }

        [HttpGet("category")]
        public async Task<ActionResult<Category>> CategoryList()
        {
            List<Category> categories = await categoryR.GetCategories();
            List<CategoryDTO> ncategories = CategoryM.ToCategoryDTO(categories);
            return Ok(categories);
        }

        [HttpGet("category/{id}")]
        public async Task<ActionResult<Category>> OneCategory(int id)
        {
            CategoryDTO category = CategoryM.ToCategoryDTO(await categoryR.GetCategory(id));
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost("add-category")]
        public async Task<ActionResult> AddCategory(CategoryDTO categorie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Category category = CategoryM.ToCategory(categorie);

            await categoryR.AddCategry(category);
            return CreatedAtAction(nameof(OneCategory), new {Id = category.Id}, category);
        }

        [HttpPost("add-produit")]
        public async Task<ActionResult<Produit>> AddProduct(ProduitVM prodvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            ProductImageDTO image = ProduitMP.ToProdImage(prodvm.ImageName);
            string imageName;
            try
            {
                imageName = await minio.UploadImage(image.ImageName);
            } catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            Produit produit = ProduitMP.ProduitVMToProduit(prodvm, imageName);
            await produitR.AddProduct(produit);
            return CreatedAtAction(nameof(Index), new { Id = produit.Id }, produit);
        }

        /*
            Actions update and delete products by admin, need kafka !!! 
        */

        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdatePoduct(IKafkaService kafka,ProduitVM produit,int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newPrice = ProduitMP.ToProdPrice(produit);
            await produitR.UpdateProductPrice(id, newPrice);

            var product = ProduitMP.ToUpdatedProductDTO(produit, id);
            await kafka.UpdatedProductPub(product);
            return NoContent();
        } 

    }
}
