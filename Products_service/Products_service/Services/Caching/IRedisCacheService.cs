namespace Products_service.Services.Caching
{
    public interface IRedisCacheService
    {
        // T after Task<T> meen the return type can be any thing => string, bool, int ...
        // The T after GetData<T> => made ref on the return type also the args passed, it specidfied 
        // when you call the function for ex => await cashe.GetData<List<PoroduitDTO>>(1) => here it meens 
        // that the return type going to be a list of ProductsDTO 
        public Task<T?> GetData<T>(string key);
        public Task SetData<T>(string key, T data, int min=5);
    }
}
