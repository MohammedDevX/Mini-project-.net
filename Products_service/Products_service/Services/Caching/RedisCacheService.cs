
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Products_service.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        // Here we use DistributedCache for scalability
        private IDistributedCache cache;
        public RedisCacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T?> GetData<T>(string key)
        {
            var data = await cache.GetStringAsync(key);
            if (data == null)
            {
                // default(T) => return the default value of the reference type who T made, it specified when
                // you call the function for ex => cash.GetData<List<ProduitDTO>>(1) =>, so here the type of 
                // T is a list of objects, so the return value going to be null, if it was a number, he will
                // going to return 0 etc ...
                return default;
            }

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetData<T>(string key, T data, int min=5)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(min)
            };

            await cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
        }
    }
}
