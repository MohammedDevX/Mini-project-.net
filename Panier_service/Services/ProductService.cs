using Microsoft.AspNetCore.Http;
using Panier_service.DTOS;
using System.Text.Json;

namespace Panier_service.Services
{
    public class ProductService
    {
        private HttpClient client;
        private IHttpContextAccessor httpContext;
        public ProductService(HttpClient client, IHttpContextAccessor httpContext)
        {
            this.client = client;
            this.httpContext = httpContext;
        }

        public async Task<double?> GetPriceProduct(int id)
        {
            try
            {
                var token = httpContext.HttpContext?
                            .Request
                            .Headers["Authorization"]
                            .ToString();

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        token.Replace("bearer ", "")
                    );

                var url = $"https://localhost:7209/produit/price/{id}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    double productPrice = JsonSerializer.Deserialize<double>(json);
                    return productPrice;
                } else
                {
                    return null;
                }

            } catch
            {
                return null;
            }
        }
    }
}
